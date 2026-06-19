// <copyright file="ReadOnlyPostgresDbContext.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database;

using System.Reflection;

/// <summary>
/// The Read-Only Authorisation DbContext.
/// </summary>
/// <param name="options">options to apply to the context.</param>
public class ReadOnlyPostgresDbContext(DbContextOptions<ReadOnlyPostgresDbContext> options)
    : PostgresDbContextBase<ReadOnlyPostgresDbContext>(options)
{
    public override int SaveChanges()
    {
        throw new InvalidOperationException("This context is read-only.");
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException("This context is read-only.");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Requires.NotNull(modelBuilder);

        modelBuilder.HasDefaultSchema(DatabaseConstants.SchemaName);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.HasPostgresExtension(PostgreExtensions.PgCrypto);
        modelBuilder.HasPostgresExtension(PostgreExtensions.Citext);
    }
}
