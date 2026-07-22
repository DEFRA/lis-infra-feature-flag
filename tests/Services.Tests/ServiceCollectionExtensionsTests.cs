// <copyright file="ServiceCollectionExtensionsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Services.Tests;

using Defra.Livestock.Sdk.Api.Strategies.Abstractions.Operations;
using Defra.Livestock.Sdk.Api.Strategies.Operations;
using Lis.Infra.FeatureFlag.Repositories.FeatureFlagStatuses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddServices_ShouldRegisterFeatureServiceAsTransient()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection([])
            .Build();
        var repository = Substitute.For<IFeatureFlagStatusRepository>();
        var logger = Substitute.For<ILogger<FeatureFlagService>>();

        services.AddSingleton(repository);
        services.AddSingleton(logger);

        var returnedServices = services.AddServices(configuration);
        using var provider = services.BuildServiceProvider();

        returnedServices.ShouldBeSameAs(services);
        provider.GetRequiredService<IFeatureFlagService>().ShouldBeOfType<FeatureFlagService>();
        provider.GetRequiredService<IRepoStrategyFactory<FeatureFlagService>>()
            .ShouldBeOfType<RepoStrategyFactory<FeatureFlagService>>();
    }
}
