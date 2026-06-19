// <copyright file="IPostgresDataSourceFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database;

using Npgsql;

public interface IPostgresDataSourceFactory
{
    NpgsqlDataSource CreateDataSource(string connectionIdentifier);
}
