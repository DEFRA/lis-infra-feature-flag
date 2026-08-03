// <copyright file="ServiceCollectionExtensionsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Tests.Extensions;

using Lis.Infra.FeatureFlag.Api.Extensions;
using Lis.Infra.FeatureFlag.Api.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceCollectionExtensions = Lis.Infra.FeatureFlag.Api.Extensions.ServiceCollectionExtensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddRequests_Registers_Services()
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
        ServiceCollectionExtensions.ApiKey.ShouldBe("test");

        serviceProvider.GetService<ApiKeyValidationMiddleware>().ShouldNotBeNull();
        serviceProvider.GetService<CorrelationIdValidationMiddleware>().ShouldNotBeNull();
    }

    [Fact]
    public void AddRequests_WithoutApiKey_ThrowsException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .Build();

        // Act
        var exception = Should.Throw<ArgumentException>(() => services.AddRequests(configuration));

        // Assert
        exception.Message.ShouldContain("DefraIdentityApiKey configuration value is missing or empty");
    }

    [Fact]
    public void UseRequests_Adds_Middlewares_To_Pipeline()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Configuration["ServiceApiKey"] = "test";
        builder.Services.AddRequests(builder.Configuration);
        var app = builder.Build();

        // Act & Assert
        Should.NotThrow(app.UseRequests);
    }
}
