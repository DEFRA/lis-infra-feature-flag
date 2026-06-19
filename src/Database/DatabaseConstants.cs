// <copyright file="DatabaseConstants.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database;

internal static class DatabaseConstants
{
    internal const string SchemaName = "public";
    internal const string ConnectionStringName = "PostgresConnection";
    internal const string ReadOnlyConnectionStringName = "ReadOnlyPostgresConnection";
}
