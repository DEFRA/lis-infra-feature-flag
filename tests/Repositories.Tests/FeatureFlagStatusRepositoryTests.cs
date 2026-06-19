// <copyright file="FeatureFlagStatusRepositoryTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Repositories.Tests;

using Lis.Infra.FeatureFlag.Database;
using Lis.Infra.FeatureFlag.Database.Domain;
using Lis.Infra.FeatureFlag.Database.Entities;
using Lis.Infra.FeatureFlag.Repositories.FeatureFlagStatuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class FeatureFlagStatusRepositoryTests
{
    [Fact]
    public async Task GetFlagsTask_ShouldReturnMatchingFlagsWithRelatedEntities()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using (var writeContext = new PostgresDbContext(CreateOptions<PostgresDbContext>(databaseName)))
        {
            var paymentsGroup = new FeatureGroups
            {
                Id = Guid.NewGuid(),
                Name = "Payments",
                Description = "Payments feature group",
            };
            var ordersGroup = new FeatureGroups
            {
                Id = Guid.NewGuid(),
                Name = "Orders",
                Description = "Orders feature group",
            };
            var production = new Environments
            {
                Id = Guid.NewGuid(),
                Name = "Prod",
            };
            var newUiFlag = new FeatureFlags
            {
                Id = Guid.NewGuid(),
                Name = "NewUi",
                GroupId = paymentsGroup.Id,
            };
            var otherFlag = new FeatureFlags
            {
                Id = Guid.NewGuid(),
                Name = "AnotherFlag",
                GroupId = ordersGroup.Id,
            };

            await writeContext.AddRangeAsync(paymentsGroup, ordersGroup, production, newUiFlag, otherFlag);
            await writeContext.AddRangeAsync(
                new FeatureFlagStatuses
                {
                    Id = Guid.NewGuid(),
                    GroupId = paymentsGroup.Id,
                    EnvironmentId = production.Id,
                    ActivationType = ActivationType.Manual,
                    ManualEnabled = true,
                    UpdatedBy = "tester",
                },
                new FeatureFlagStatuses
                {
                    Id = Guid.NewGuid(),
                    GroupId = paymentsGroup.Id,
                    FlagId = newUiFlag.Id,
                    EnvironmentId = production.Id,
                    ActivationType = ActivationType.Manual,
                    ManualEnabled = true,
                    UpdatedBy = "tester",
                },
                new FeatureFlagStatuses
                {
                    Id = Guid.NewGuid(),
                    GroupId = ordersGroup.Id,
                    FlagId = otherFlag.Id,
                    ActivationType = ActivationType.Manual,
                    ManualEnabled = true,
                    UpdatedBy = "tester",
                });

            await writeContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        await using var context = new PostgresDbContext(CreateOptions<PostgresDbContext>(databaseName));
        await using var readOnlyContext = new ReadOnlyPostgresDbContext(CreateOptions<ReadOnlyPostgresDbContext>(databaseName));
        var logger = Substitute.For<ILogger<FeatureFlagStatusRepository>>();
        var sut = new FeatureFlagStatusRepository(context, readOnlyContext, logger);

        var result = await sut.GetFlagsTask(x => x.Group.Name == "Payments", TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result.All(status => status.Group.Name == "Payments").ShouldBeTrue();
        result.Single(status => status.Flag == null).Environment!.Name.ShouldBe("Prod");
        result.Single(status => status.Flag != null).Flag!.Name.ShouldBe("NewUi");
    }

    private static DbContextOptions<TContext> CreateOptions<TContext>(string databaseName)
        where TContext : DbContext
    {
        return new DbContextOptionsBuilder<TContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
    }
}
