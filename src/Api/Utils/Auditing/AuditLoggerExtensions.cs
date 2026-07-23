// <copyright file="AuditLoggerExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Utils.Auditing;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public static class AuditLoggerExtensions
{
    private static readonly Dictionary<string, object> SAuditLogLevel = new()
    {
        [AuditLogger.AuditPropertyName] = true,
    };

    public static void Audit(
        this ILogger logger,
        string message,
        params object?[] args)
    {
        ArgumentNullException.ThrowIfNull(logger);
        using (logger.BeginScope(SAuditLogLevel))
        {
            logger.LogInformation(message, args);
        }
    }

    public static void Audit(
        this ILogger logger,
        Exception exception,
        string message,
        params object?[] args)
    {
        ArgumentNullException.ThrowIfNull(logger);
        using (logger.BeginScope(SAuditLogLevel))
        {
            logger.LogInformation(exception, message, args);
        }
    }
}
