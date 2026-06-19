// <copyright file="IPostgresIamTokenGeneratorService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database;

public interface IPostgresIamTokenGeneratorService
{
    Task<string> GenerateAuthTokenAsync(string hostname, int port, string username);
}
