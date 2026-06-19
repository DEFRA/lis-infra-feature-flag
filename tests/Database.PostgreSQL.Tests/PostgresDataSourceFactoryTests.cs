// <copyright file="PostgresDataSourceFactoryTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database.PostgreSql.Tests;

using Lis.Infra.FeatureFlag.Database.Domain;
using Lis.Infra.FeatureFlag.Database.Entities;
using NSubstitute;

public class PostgresDataSourceFactoryTests
{
    [Fact]
    public void CreateDataSource_ShouldCacheDataSourcesByIdentifier()
    {
        using var sut = new PostgresDataSourceFactory(
            new PostgresConfiguration
            {
                ConnectionString = "Host=localhost;Database=feature_flags;Username=test;Password=test",
            });

        var first = sut.CreateDataSource("Default");
        var second = sut.CreateDataSource("Default");

        second.ShouldBeSameAs(first);
    }

    [Fact]
    public void CreateDataSource_ShouldFallbackToDefaultConnectionString_ForReadOnlyConnection()
    {
        using var sut = new PostgresDataSourceFactory(
            new PostgresConfiguration
            {
                ConnectionString = "Host=localhost;Database=feature_flags;Username=test;Password=test",
            });

        var dataSource = sut.CreateDataSource("ReadOnly");

        dataSource.ShouldNotBeNull();
    }

    [Fact]
    public void CreateDataSource_ShouldThrow_WhenDefaultConnectionStringIsMissing()
    {
        using var sut = new PostgresDataSourceFactory(new PostgresConfiguration());

        var exception = Should.Throw<InvalidOperationException>(() => sut.CreateDataSource("Default"));

        exception.Message.ShouldContain("Connection string for 'Default' is missing.");
    }

    [Fact]
    public void CreateDataSource_ShouldThrow_WhenIdentifierIsUnknown()
    {
        using var sut = new PostgresDataSourceFactory(
            new PostgresConfiguration
            {
                ConnectionString = "Host=localhost;Database=feature_flags;Username=test;Password=test",
            });

        var exception = Should.Throw<ArgumentException>(() => sut.CreateDataSource("Other"));

        exception.Message.ShouldContain("Unknown connection identifier");
    }

    [Fact]
    public void CreateDataSource_ShouldThrow_WhenFactoryIsDisposed()
    {
        var sut = new PostgresDataSourceFactory(
            new PostgresConfiguration
            {
                ConnectionString = "Host=localhost;Database=feature_flags;Username=test;Password=test",
            });

        sut.Dispose();

        Should.Throw<ObjectDisposedException>(() => sut.CreateDataSource("Default"));
    }

    [Fact]
    public void CreateDataSource_ShouldCreateIamAuthenticatedDataSource_WhenEnabled()
    {
        var tokenGenerator = Substitute.For<IPostgresIamTokenGeneratorService>();
        using var sut = new PostgresDataSourceFactory(
            new PostgresConfiguration
            {
                UseIamAuthentication = true,
                DefaultHost = "writer.example.com",
                ReadOnlyHost = "reader.example.com",
                Port = 5432,
                Name = "feature_flags",
                User = "app-user",
            },
            tokenGenerator);

        var dataSource = sut.CreateDataSource("Default");

        dataSource.ShouldNotBeNull();
    }

    [Fact]
    public async Task PostgresIamTokenGeneratorService_ShouldGenerateToken()
    {
        var sut = new PostgresIamTokenGeneratorService(
            new Amazon.Runtime.BasicAWSCredentials("access-key", "secret-key"),
            Amazon.RegionEndpoint.EUWest2);

        var token = await sut.GenerateAuthTokenAsync("writer.example.com", 5432, "app-user");

        token.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void DatabaseEntities_ShouldExposeAssignedValues()
    {
        var group = new FeatureGroups
        {
            Id = Guid.NewGuid(),
            Name = "Payments",
            Description = "Payment features",
        };
        var flag = new FeatureFlags
        {
            Id = Guid.NewGuid(),
            Name = "NewUi",
            Description = "New UI feature",
            GroupId = group.Id,
            Group = group,
        };
        var environment = new Environments
        {
            Id = Guid.NewGuid(),
            Name = "Prod",
            Description = "Production",
        };
        var status = new FeatureFlagStatuses
        {
            Id = Guid.NewGuid(),
            GroupId = group.Id,
            FlagId = flag.Id,
            EnvironmentId = environment.Id,
            ActivationType = ActivationType.Manual,
            ManualEnabled = true,
            ActivateAfter = DateTime.UtcNow.AddMinutes(-1),
            ExpireAt = DateTime.UtcNow.AddMinutes(1),
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = "tester",
            Group = group,
            Flag = flag,
            Environment = environment,
        };

        environment.FeatureFlagStatuses.Add(status);
        group.FeatureFlags.Add(flag);
        group.FeatureFlagStatuses.Add(status);
        flag.FeatureFlagStatuses.Add(status);

        group.Name.ShouldBe("Payments");
        group.Description.ShouldBe("Payment features");
        group.FeatureFlags.ShouldContain(flag);
        group.FeatureFlagStatuses.ShouldContain(status);
        flag.Name.ShouldBe("NewUi");
        flag.Description.ShouldBe("New UI feature");
        flag.GroupId.ShouldBe(group.Id);
        flag.Group.ShouldBeSameAs(group);
        flag.FeatureFlagStatuses.ShouldContain(status);
        environment.Name.ShouldBe("Prod");
        environment.Description.ShouldBe("Production");
        environment.FeatureFlagStatuses.ShouldContain(status);
        status.GroupId.ShouldBe(group.Id);
        status.FlagId.ShouldBe(flag.Id);
        status.EnvironmentId.ShouldBe(environment.Id);
        status.ActivationType.ShouldBe(ActivationType.Manual);
        status.ManualEnabled.ShouldBe(true);
        status.UpdatedBy.ShouldBe("tester");
        status.Group.ShouldBeSameAs(group);
        status.Flag.ShouldBeSameAs(flag);
        status.Environment.ShouldBeSameAs(environment);
    }
}
