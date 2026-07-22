// <copyright file="RequestHeaderNames.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Middleware.Headers;

public static class RequestHeaderNames
{
    public const string CorrelationId = "x-correlation-id";
    public const string ApiKey = "x-api-key";

    public const string ProductName = "product-name";
    public const string EnvironmentName = "environment-name";
}
