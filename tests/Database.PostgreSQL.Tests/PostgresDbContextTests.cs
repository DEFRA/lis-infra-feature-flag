// <copyright file="PostgresDbContextTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database.PostgreSQL.Tests;

using Lis.Infra.FeatureFlag.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

public class PostgresDbContextTests
{
    [Fact]
    public void PostgresDbContext_ShouldApplyExpectedModelConfiguration()
    {
        using var context = new PostgresDbContext(CreateOptions<PostgresDbContext>());
        var designTimeModel = context.GetService<IDesignTimeModel>().Model;

        context.Model.GetDefaultSchema().ShouldBe("public");
        context.Environments.ShouldNotBeNull();
        context.Products.ShouldNotBeNull();
        context.FeatureGroups.ShouldNotBeNull();
        context.FeatureFlags.ShouldNotBeNull();
        context.FeatureFlagStatuses.ShouldNotBeNull();

        var statusEntity = designTimeModel.FindEntityType(typeof(FeatureFlagStatuses));
        statusEntity.ShouldNotBeNull();
        statusEntity!.GetTableName().ShouldBe("feature_flag_statuses");
        statusEntity.GetCheckConstraints().Count().ShouldBe(3);
        statusEntity.FindProperty(nameof(FeatureFlagStatuses.UpdatedBy))!.GetColumnType().ShouldBe(ColumnTypes.Text);
        statusEntity.FindProperty(nameof(FeatureFlagStatuses.UpdatedAt))!.GetColumnType().ShouldBe(ColumnTypes.DateTimeOffSet);
        statusEntity.GetForeignKeys().Select(foreignKey => foreignKey.PrincipalEntityType.ClrType)
            .OrderBy(type => type.Name)
            .ShouldBe([typeof(Environments), typeof(FeatureFlags), typeof(FeatureGroups), typeof(Products)]);

        var productEntity = designTimeModel.FindEntityType(typeof(Products));
        productEntity.ShouldNotBeNull();
        productEntity!.GetTableName().ShouldBe("products");

        var groupEntity = designTimeModel.FindEntityType(typeof(FeatureGroups));
        groupEntity.ShouldNotBeNull();
        groupEntity!.GetTableName().ShouldBe("feature_groups");
        groupEntity.GetForeignKeys().Single().PrincipalEntityType.ClrType.ShouldBe(typeof(Products));

        var flagEntity = designTimeModel.FindEntityType(typeof(FeatureFlags));
        flagEntity.ShouldNotBeNull();
        flagEntity!.GetTableName().ShouldBe("feature_flags");
        flagEntity.GetForeignKeys().Select(foreignKey => foreignKey.PrincipalEntityType.ClrType)
            .OrderBy(type => type.Name)
            .ShouldBe([typeof(FeatureGroups), typeof(Products)]);

        designTimeModel.GetEntityTypes().Select(entity => entity.GetTableName())
            .ShouldContain("environments");
        designTimeModel.GetEntityTypes().Select(entity => entity.GetTableName())
            .ShouldContain("products");
        designTimeModel.GetEntityTypes().Select(entity => entity.GetTableName())
            .ShouldContain("feature_groups");
    }

    [Fact]
    public void ReadOnlyPostgresDbContext_ShouldThrow_WhenSavingSynchronously()
    {
        using var context = new ReadOnlyPostgresDbContext(CreateOptions<ReadOnlyPostgresDbContext>());

        var exception = Should.Throw<InvalidOperationException>(() => context.SaveChanges());

        exception.Message.ShouldBe("This context is read-only.");
    }

    [Fact]
    public async Task ReadOnlyPostgresDbContext_ShouldThrow_WhenSavingAsynchronously()
    {
        await using var context = new ReadOnlyPostgresDbContext(CreateOptions<ReadOnlyPostgresDbContext>());

        var exception = await Should.ThrowAsync<InvalidOperationException>(() => context.SaveChangesAsync());

        exception.Message.ShouldBe("This context is read-only.");
    }

    private static DbContextOptions<TContext> CreateOptions<TContext>()
        where TContext : DbContext
    {
        return new DbContextOptionsBuilder<TContext>()
            .UseNpgsql("Host=localhost;Database=feature_flags;Username=test;Password=test")
            .Options;
    }
}
