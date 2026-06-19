// <copyright file="PostgresConfiguration.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database;

public class PostgresConfiguration
{
    public bool UseIamAuthentication { get; init; } = false;

    public string DefaultHost { get; init; } = string.Empty;

    public string ReadOnlyHost { get; init; } = string.Empty;

    public int Port { get; init; } = 5432;

    public string Name { get; init; } = string.Empty;

    public string User { get; init; } = string.Empty;

    public string ConnectionString { get; set; } = string.Empty;

    public string ReadOnlyConnectionString { get; set; } = string.Empty;
}
