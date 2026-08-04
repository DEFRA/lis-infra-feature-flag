// <copyright file="ApiKeyValidationMiddlewareTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Tests.Middleware;

using Lis.Infra.FeatureFlag.Api.Extensions;
using Lis.Infra.FeatureFlag.Api.MetaData;
using Lis.Infra.FeatureFlag.Api.Middleware;
using Lis.Infra.FeatureFlag.Api.Middleware.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class ApiKeyValidationMiddlewareTests
{
    private readonly ILogger<ApiKeyValidationMiddleware> logger = Substitute.For<ILogger<ApiKeyValidationMiddleware>>();

    [Fact]
    public void AddRequests_RegistersMiddleware_CanBeResolved()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { { "ServiceApiKey", "test" }, })
            .Build();

        // Act
        services.AddLogging();
        services.AddRequests(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var middleware = serviceProvider.GetService<ApiKeyValidationMiddleware>();
        middleware.ShouldNotBeNull();
    }

    [Fact]
    public void UseRequests_WithIMiddleware_DoesNotThrow()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Configuration["ServiceApiKey"] = "test";
        builder.Services.AddLogging();
        builder.Services.AddRequests(builder.Configuration);
        var app = builder.Build();

        // Act & Assert
        Should.NotThrow(app.UseRequests);
    }

    [Fact]
    public async Task UseRequests_WithNoEndpoint_ReturnsWithoutProcessing()
    {
        // Arrange
        var middleware = new ApiKeyValidationMiddleware("test", logger);
        var context = new DefaultHttpContext();
        context.Request.Headers[RequestHeaderNames.ApiKey] = "test";

        // Act
        await middleware.InvokeAsync(context, (_) => Task.CompletedTask);

        // Assert
        context.Response.StatusCode.ShouldBe(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task UseRequests_WithIgnoreKey_ReturnsWithoutProcessing()
    {
        // Arrange
        var middleware = new ApiKeyValidationMiddleware("test", logger);
        var endpoint = Substitute.For<IEndpointFeature>();
        endpoint.Endpoint =
            new Endpoint(null, new EndpointMetadataCollection(new IgnoreApiKeyCheck()), "fake endpoint");
        var context = new DefaultHttpContext();
        context.Request.Headers[RequestHeaderNames.ApiKey] = "test";
        context.Features.Set(endpoint);

        // Act
        await middleware.InvokeAsync(context, (_) => Task.CompletedTask);

        // Assert
        context.Response.StatusCode.ShouldBe(StatusCodes.Status200OK);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    public async Task UseRequests_WithMissingKey_ReturnsErrorJson(string keyValue)
    {
        // Arrange
        var middleware = new ApiKeyValidationMiddleware("test", logger);
        var endpoint = Substitute.For<IEndpointFeature>();
        endpoint.Endpoint = new Endpoint(null, null, "fake endpoint");
        var context = new DefaultHttpContext();
        context.Request.Headers[RequestHeaderNames.ApiKey] = keyValue;
        context.Features.Set(endpoint);

        // Act
        await middleware.InvokeAsync(context, (_) => Task.CompletedTask);

        // Assert
        context.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionThrown_LogsErrorAndReThrows()
    {
        // Arrange
        var middleware = new ApiKeyValidationMiddleware("test", logger);

        var endpoint = Substitute.For<IEndpointFeature>();
        endpoint.Endpoint =
            new Endpoint(null, new EndpointMetadataCollection(), "fake endpoint");

        var context = new DefaultHttpContext();
        context.Request.Headers[RequestHeaderNames.ApiKey] = "test";
        context.Features.Set(endpoint);

        var exception = new Exception("Test exception");
        RequestDelegate next = (_) => throw exception;

        // Act & Assert
        var ex = await Should.ThrowAsync<Exception>(() => middleware.InvokeAsync(context, next));
        ex.ShouldBe(exception);
    }
}
