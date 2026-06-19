// <copyright file="PostgresIamTokenGeneratorService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database;

using Amazon;
using Amazon.RDS.Util;
using Amazon.Runtime;

public class PostgresIamTokenGeneratorService(AWSCredentials credentials, RegionEndpoint region) : IPostgresIamTokenGeneratorService
{
    public Task<string> GenerateAuthTokenAsync(string hostname, int port, string username)
    {
        var token = RDSAuthTokenGenerator.GenerateAuthToken(credentials, region, hostname, port, username);
        return Task.FromResult(token);
    }
}
