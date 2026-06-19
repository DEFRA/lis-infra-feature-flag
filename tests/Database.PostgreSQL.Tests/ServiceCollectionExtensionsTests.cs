// <copyright file="ServiceCollectionExtensionsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database.PostgreSql.Tests;

using Amazon.Runtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddPostgresDatabase_ShouldRegisterConfigurationAndContexts()
    {
        var configuration = BuildConfiguration(useIamAuthentication: false);
        var services = new ServiceCollection();

        services.AddLogging();

        var returnedServices = services.AddPostgresDatabase(configuration);
        using var provider = services.BuildServiceProvider();

        var postgresConfiguration = provider.GetRequiredService<PostgresConfiguration>();
        var readOnlyContext = provider.GetRequiredService<ReadOnlyPostgresDbContext>();

        returnedServices.ShouldBeSameAs(services);
        postgresConfiguration.ConnectionString.ShouldBe("Host=localhost;Database=feature_flags;Username=test;Password=test");
        postgresConfiguration.ReadOnlyConnectionString.ShouldBe("Host=localhost;Database=feature_flags_read;Username=test;Password=test");
        provider.GetRequiredService<IPostgresDataSourceFactory>().ShouldBeOfType<PostgresDataSourceFactory>();
        provider.GetRequiredService<PostgresDbContext>().ShouldNotBeNull();
        readOnlyContext.ChangeTracker.QueryTrackingBehavior.ShouldBe(QueryTrackingBehavior.NoTracking);
        provider.GetService<IPostgresIamTokenGeneratorService>().ShouldBeNull();
    }

    [Fact]
    public void AddPostgresDatabase_ShouldRegisterIamTokenGenerator_WhenIamAuthIsEnabled()
    {
        var configuration = BuildConfiguration(useIamAuthentication: true);
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddSingleton<AWSCredentials>(new BasicAWSCredentials("access-key", "secret-key"));
        services.AddPostgresDatabase(configuration);

        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IPostgresIamTokenGeneratorService>()
            .ShouldBeOfType<PostgresIamTokenGeneratorService>();
    }

    private static IConfigurationRoot BuildConfiguration(bool useIamAuthentication)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                ["PostgresConfiguration:UseIamAuthentication"] = useIamAuthentication.ToString(),
                ["PostgresConfiguration:DefaultHost"] = "writer.example.com",
                ["PostgresConfiguration:ReadOnlyHost"] = "reader.example.com",
                ["PostgresConfiguration:Port"] = "5432",
                ["PostgresConfiguration:Name"] = "feature_flags",
                ["PostgresConfiguration:User"] = "app-user",
                ["ConnectionStrings:PostgresConnection"] = "Host=localhost;Database=feature_flags;Username=test;Password=test",
                ["ConnectionStrings:ReadOnlyPostgresConnection"] = "Host=localhost;Database=feature_flags_read;Username=test;Password=test",
                ["AWS:Region"] = "eu-west-2",
            })
            .Build();
    }
}
