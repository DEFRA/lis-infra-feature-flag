// <copyright file="PostgresDataSourceFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database;

using System.Diagnostics.CodeAnalysis;
using Npgsql;

public sealed class PostgresDataSourceFactory(PostgresConfiguration config, IPostgresIamTokenGeneratorService? iamTokenGenerator = null) : IPostgresDataSourceFactory, IDisposable
{
    private const string DefaultConnectionIdentifier = "Default";
    private const string ReadOnlyConnectionIdentifier = "ReadOnly";

    private readonly Dictionary<string, NpgsqlDataSource> dataSources = new();
    private readonly SemaphoreSlim @lock = new(1, 1);
    private bool disposed;

    public NpgsqlDataSource CreateDataSource(string connectionIdentifier)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        // Use cached data source if available
        if (dataSources.TryGetValue(connectionIdentifier, out var existingDataSource))
        {
            return existingDataSource;
        }

        @lock.Wait();
        try
        {
            // Double-check after acquiring lock
            if (dataSources.TryGetValue(connectionIdentifier, out existingDataSource))
            {
                return existingDataSource;
            }

            NpgsqlDataSource dataSource;

            if (config.UseIamAuthentication)
            {
                dataSource = CreateIamAuthDataSource(connectionIdentifier);
            }
            else
            {
                dataSource = CreateStandardDataSource(connectionIdentifier);
            }

            dataSources[connectionIdentifier] = dataSource;
            return dataSource;
        }
        finally
        {
            @lock.Release();
        }
    }

    private NpgsqlDataSource CreateStandardDataSource(string connectionIdentifier)
    {
        var connectionString = connectionIdentifier switch
        {
            DefaultConnectionIdentifier => config.ConnectionString,
            ReadOnlyConnectionIdentifier => config.ReadOnlyConnectionString,
            _ => throw new ArgumentException($"Unknown connection identifier: {connectionIdentifier}"),
        };

        if (string.IsNullOrEmpty(connectionString) && connectionIdentifier == ReadOnlyConnectionIdentifier)
        {
            connectionString = config.ConnectionString;
        }

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Connection string for '{connectionIdentifier}' is missing.");
        }

        return NpgsqlDataSource.Create(connectionString);
    }

    private NpgsqlDataSource CreateIamAuthDataSource(string connectionIdentifier)
    {
        var host = connectionIdentifier switch
        {
            DefaultConnectionIdentifier => config.DefaultHost,
            ReadOnlyConnectionIdentifier => config.ReadOnlyHost,
            _ => throw new ArgumentException($"Unknown connection identifier: {connectionIdentifier}"),
        };

        var builder = new NpgsqlDataSourceBuilder
        {
            ConnectionStringBuilder =
            {
                Host = host,
                Port = config.Port,
                Database = config.Name,
                Username = config.User,
                SslMode = SslMode.Require, // AWS RDS requires SSL
            },
        };

        // Register password provider that generates IAM tokens
        builder.UsePeriodicPasswordProvider(
            passwordProvider: async (_, ct) =>
            {
                var token = await iamTokenGenerator!.GenerateAuthTokenAsync(
                    host,
                    config.Port,
                    config.User!);
                return token;
            },
            successRefreshInterval: TimeSpan.FromMinutes(10), // Refresh every 10 minutes
            failureRefreshInterval: TimeSpan.FromSeconds(30)); // Retry after 30 seconds on failure

        return builder.Build();
    }

    [ExcludeFromCodeCoverage]
    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        foreach (var dataSource in dataSources.Values)
        {
            dataSource.Dispose();
        }

        dataSources.Clear();
        @lock.Dispose();

        disposed = true;
    }
}
