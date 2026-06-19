// <copyright file="PostgresDbContext.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database;

using System.Reflection;

/// <summary>
/// The Read-Write Authorisation DbContext.
/// </summary>
/// <param name="options">options to apply to the context.</param>
public class PostgresDbContext(DbContextOptions<PostgresDbContext> options)
    : PostgresDbContextBase<PostgresDbContext>(options)
{
    protected virtual void ConfigureModel(ModelBuilder modelBuilder)
    {
        Requires.NotNull(modelBuilder);

        modelBuilder.HasDefaultSchema(DatabaseConstants.SchemaName);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.HasPostgresExtension(PostgreExtensions.PgCrypto);
        modelBuilder.HasPostgresExtension(PostgreExtensions.Citext);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureModel(modelBuilder);
    }
}
