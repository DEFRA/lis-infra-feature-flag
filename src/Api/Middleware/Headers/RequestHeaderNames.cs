// <copyright file="RequestHeaderNames.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Middleware.Headers;

public static class RequestHeaderNames
{
    public const string CorrelationId = "x-correlation-id";
    public const string OperatorId = "x-operator-id";
    public const string ApiKey = "x-api-key";
}
