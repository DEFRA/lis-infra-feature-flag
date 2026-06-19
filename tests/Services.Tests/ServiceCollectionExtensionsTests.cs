// <copyright file="ServiceCollectionExtensionsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Services.Tests;

using Lis.Infra.FeatureFlag.Repositories.FeatureFlagStatuses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        services.AddSingleton(repository);

        var returnedServices = services.AddServices(configuration);
        using var provider = services.BuildServiceProvider();

        returnedServices.ShouldBeSameAs(services);
        provider.GetRequiredService<IFeatureService>().ShouldBeOfType<FeatureService>();
        provider.GetRequiredService<IFeatureService>().ShouldNotBeSameAs(provider.GetRequiredService<IFeatureService>());
    }
}
