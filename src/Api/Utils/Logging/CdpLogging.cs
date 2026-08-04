// <copyright file="CdpLogging.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Utils.Logging;

using System.Diagnostics.CodeAnalysis;
using Elastic.Serilog.Enrichers.Web;
using Lis.Infra.FeatureFlag.Api.Middleware.Headers;
using Lis.Infra.FeatureFlag.Api.Utils.Auditing;
using Serilog;

public static class CdpLogging
{
    [ExcludeFromCodeCoverage]
    public static void Configuration(HostBuilderContext ctx, LoggerConfiguration config)
    {
        var httpAccessor = ctx.Configuration.Get<HttpContextAccessor>();

        var mainLogger = new LoggerConfiguration()
            .ReadFrom.Configuration(ctx.Configuration)
            .Enrich.WithEcsHttpContext(httpAccessor!)
            .Enrich.FromLogContext()
            .Filter.With<AuditLogger.Filters.ExcludeAuditEvents>()
            .CreateLogger();

        config.Enrich.WithCorrelationId(RequestHeaderNames.CorrelationId);

        var auditLogger = AuditLogger.CreateAuditLogger();

        config
            .WriteTo.Logger(mainLogger)
            .WriteTo.Logger(auditLogger);
    }
}
