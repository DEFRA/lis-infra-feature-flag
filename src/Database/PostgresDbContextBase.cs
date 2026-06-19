// <copyright file="PostgresDbContextBase.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database;

public abstract class PostgresDbContextBase<T>(DbContextOptions<T> options) : DbContext(options)
    where T : DbContext
{
    public virtual DbSet<Environments> Environments { get; set; }

    public virtual DbSet<FeatureGroups> FeatureGroups { get; set; }

    public virtual DbSet<FeatureFlags> FeatureFlags { get; set; }

    public virtual DbSet<FeatureFlagStatuses> FeatureFlagStatuses { get; set; }
}
