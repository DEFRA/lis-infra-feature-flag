// <copyright file="CdpLogging.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Utils.Logging;

using System.Diagnostics.CodeAnalysis;
using Elastic.Serilog.Enrichers.Web;
using Lis.Infra.FeatureFlag.Api.Middleware.Headers;
using Lis.Infra.FeatureFlag.Api.Utils.Auditing;
using Serilog;
using Serilog.Events;

public static class CdpLogging
{
    private const string MinimumLevelSection = "Serilog:MinimumLevel";

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

        // Serilog applies MinimumLevel overrides where an event is created, never in a sub-logger, so the
        // Serilog:MinimumLevel settings must be read onto this logger too or the overrides do nothing (LREG-899).
        config.ReadFrom.Configuration(RootMinimumLevelSettings(ctx.Configuration));

        var auditLogger = AuditLogger.CreateAuditLogger();

        config
            .WriteTo.Logger(mainLogger)
            .WriteTo.Logger(auditLogger);
    }

    /// <summary>
    /// The Serilog:MinimumLevel settings for the root logger, with the default level capped at Information. Audit
    /// events are logged at Information and must still reach the audit logger; the main logger applies the configured
    /// default itself, so a default of Warning still quietens ordinary logs. An override above Information for a source
    /// that writes audit events would drop them, so set overrides only for sources that do not audit.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>A configuration holding only the root logger's minimum level settings.</returns>
    public static IConfiguration RootMinimumLevelSettings(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(MinimumLevelSection);
        var settings = section.AsEnumerable().ToDictionary(setting => setting.Key, setting => setting.Value);

        // Serilog accepts the default either as the section's own value or as its Default key.
        foreach (var key in new[] { MinimumLevelSection, $"{MinimumLevelSection}:Default" })
        {
            if (Enum.TryParse<LogEventLevel>(configuration[key], ignoreCase: true, out var level) &&
                level > LogEventLevel.Information)
            {
                settings[key] = nameof(LogEventLevel.Information);
            }
        }

        return new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
    }
}
