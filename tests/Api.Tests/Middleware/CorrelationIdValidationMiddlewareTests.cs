// <copyright file="CorrelationIdValidationMiddlewareTests.cs" company="Defra">
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

public class CorrelationIdValidationMiddlewareTests
{
    private readonly ILogger<CorrelationIdValidationMiddleware> logger =
        Substitute.For<ILogger<CorrelationIdValidationMiddleware>>();

    [Fact]
    public void AddRequests_RegistersMiddleware_CanBeResolved()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?> { { "ServiceApiKey", "test" }, })
            .Build();

        // Act
        services.AddLogging();
        services.AddRequests(configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var middleware = serviceProvider.GetService<CorrelationIdValidationMiddleware>();
        middleware.ShouldNotBeNull();
    }

    [Fact]
    public void UseRequests_WithIMiddleware_DoesNotThrow()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Configuration["ServiceApiKey"] = "test";
        builder.Services.AddRequests(builder.Configuration);
        var app = builder.Build();

        // Act & Assert
        Should.NotThrow(app.UseRequests);
    }

    [Fact]
    public async Task UseRequests_WithNoEndpoint_ReturnsWithoutProcessing()
    {
        // Arrange
        var middleware = new CorrelationIdValidationMiddleware(logger);
        var context = new DefaultHttpContext();
        context.Request.Headers[RequestHeaderNames.ApiKey] = "test";

        // Act
        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        // Assert
        context.Response.StatusCode.ShouldBe(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task UseRequests_WithIgnoreKey_ReturnsWithoutProcessing()
    {
        // Arrange
        var middleware = new CorrelationIdValidationMiddleware(logger);

        var endpoint = Substitute.For<IEndpointFeature>();

        endpoint.Endpoint = new Endpoint(
            null,
            new EndpointMetadataCollection(new IgnoreCorrelationIdCheck()),
            "fake endpoint");

        var context = new DefaultHttpContext();
        context.Features.Set(endpoint);

        // Act
        await middleware.InvokeAsync(context, (_) => Task.CompletedTask);

        // Assert
        context.Response.StatusCode.ShouldBe(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task InvokeAsync_WithCorrelationIdHeader_CallsNext()
    {
        // Arrange
        var middleware =
            new CorrelationIdValidationMiddleware(Substitute.For<ILogger<CorrelationIdValidationMiddleware>>());
        var endpoint = Substitute.For<IEndpointFeature>();
        endpoint.Endpoint = new Endpoint(null, null, "fake endpoint");
        var context = new DefaultHttpContext();
        context.Request.Headers[RequestHeaderNames.CorrelationId] = "test-correlation-id";
        context.Features.Set(endpoint);

        var nextCalled = false;

        // Act
        await middleware.InvokeAsync(context, Next);

        // Assert
        nextCalled.ShouldBeTrue();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status200OK);
        return;

        Task Next(HttpContext ctx)
        {
            nextCalled = true;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task InvokeAsync_MissingCorrelationIdHeader_ReturnsBadRequest()
    {
        // Arrange
        var middleware =
            new CorrelationIdValidationMiddleware(Substitute.For<ILogger<CorrelationIdValidationMiddleware>>());
        var endpoint = Substitute.For<IEndpointFeature>();
        endpoint.Endpoint = new Endpoint(null, null, "fake endpoint");
        var context = new DefaultHttpContext { Response = { Body = new MemoryStream() } };
        context.Features.Set(endpoint);

        var nextCalled = false;

        // Act
        await middleware.InvokeAsync(context, Next);

        // Assert
        nextCalled.ShouldBeFalse();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        context.Response.ContentType.ShouldBe("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);
        responseBody.ShouldContain("missing_header");
        responseBody.ShouldContain($"Header {RequestHeaderNames.CorrelationId} is required.");
        return;

        Task Next(HttpContext ctx)
        {
            nextCalled = true;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task InvokeAsync_WhitespaceCorrelationIdHeader_ReturnsBadRequest()
    {
        // Arrange
        var middleware =
            new CorrelationIdValidationMiddleware(Substitute.For<ILogger<CorrelationIdValidationMiddleware>>());
        var endpoint = Substitute.For<IEndpointFeature>();
        endpoint.Endpoint = new Endpoint(null, null, "fake endpoint");
        var context = new DefaultHttpContext();
        context.Request.Headers[RequestHeaderNames.CorrelationId] = "   ";
        context.Response.Body = new MemoryStream();
        context.Features.Set(endpoint);

        var nextCalled = false;

        // Act
        await middleware.InvokeAsync(context, Next);

        // Assert
        nextCalled.ShouldBeFalse();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        context.Response.ContentType.ShouldBe("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);
        responseBody.ShouldContain("missing_header");
        responseBody.ShouldContain($"Header {RequestHeaderNames.CorrelationId} is required.");
        return;

        Task Next(HttpContext ctx)
        {
            nextCalled = true;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionThrown_LogsErrorAndReThrows()
    {
        // Arrange
        var middleware = new CorrelationIdValidationMiddleware(logger);
        var endpoint = Substitute.For<IEndpointFeature>();
        endpoint.Endpoint = new Endpoint(null, null, "fake endpoint");
        var context = new DefaultHttpContext();
        context.Request.Headers[RequestHeaderNames.CorrelationId] = "test-correlation-id";
        context.Features.Set(endpoint);
        var exception = new Exception("Test exception");
        RequestDelegate next = (_) => throw exception;

        // Act & Assert
        var ex = await Should.ThrowAsync<Exception>(() => middleware.InvokeAsync(context, next));
        ex.ShouldBe(exception);
    }
}
