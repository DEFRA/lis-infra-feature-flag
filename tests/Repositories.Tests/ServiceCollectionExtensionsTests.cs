// <copyright file="ServiceCollectionExtensionsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Repositories.Tests;

using Lis.Infra.FeatureFlag.Database;
using Lis.Infra.FeatureFlag.Repositories.FeatureFlagStatuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddRepositories_ShouldRegisterFeatureFlagStatusRepository()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection([])
            .Build();

        services.AddLogging();
        services.AddSingleton(new PostgresConfiguration
        {
            ConnectionString = "Host=localhost;Database=feature_flags;Username=test;Password=test",
        });
        services.AddSingleton<IPostgresDataSourceFactory>(
            _ => new PostgresDataSourceFactory(
                new PostgresConfiguration
                {
                    ConnectionString = "Host=localhost;Database=feature_flags;Username=test;Password=test",
                }));
        services.AddDbContext<PostgresDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddDbContext<ReadOnlyPostgresDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        var returnedServices = services.AddRepositories(configuration);
        using var provider = services.BuildServiceProvider();

        returnedServices.ShouldBeSameAs(services);
        provider.GetRequiredService<IFeatureFlagStatusRepository>()
            .ShouldBeOfType<FeatureFlagStatusRepository>();
    }
}
