// <copyright file="PostgresDbContextFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database;

using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Design;

/// <summary>
///     This is a factory class that is used by the EF Core tools to create an instance of the application db
///     context only for the purpose of generating the migrations.
///     We never use it to create an instance of the db context
///     for the application to use.
///      The connection string it purposefully set to a dummy value that will never be used
///     and it is not meant to be pointed to any real database or Local DB instance.
///     Intentionally excluded from Code Coverage because this is just for development use
/// </summary>
[ExcludeFromCodeCoverage]
internal class PostgresDbContextFactory : IDesignTimeDbContextFactory<PostgresDbContext>
{
    private const string LocalBuild = "LocalBuild";

    public PostgresDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<PostgresDbContext> dbContextOptionsBuilder =
            new();

        dbContextOptionsBuilder.UseNpgsql(LocalBuild);
        return new PostgresDbContext(dbContextOptionsBuilder.Options);
    }
}
