// <copyright file="ApiKeyValidationMiddleware.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Middleware;

public partial class ApiKeyValidationMiddleware
{
    [LoggerMessage(LogLevel.Error, "Error in {MiddlewareName}")]
    static partial void LogErrorInMiddleware(ILogger logger, string middlewareName, Exception exception);
}
