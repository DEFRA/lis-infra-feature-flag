// <copyright file="Program.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Lis.Infra.FeatureFlag.Api.Endpoints.Public;
using Lis.Infra.FeatureFlag.Api.Exceptions;
using Lis.Infra.FeatureFlag.Api.Utils.Logging;
using Lis.Infra.FeatureFlag.Database;
using Lis.Infra.FeatureFlag.Models;
using Lis.Infra.FeatureFlag.Repositories;
using Lis.Infra.FeatureFlag.Services;
using Serilog;

[ExcludeFromCodeCoverage]
public class Program
{
    protected Program()
    {
    }

    public static async Task Main(string[] args)
    {
        var app = CreateWebApplication(args);
        await app.RunAsync();
    }

    private static WebApplication CreateWebApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile(
                $"appsettings.{builder.Environment.EnvironmentName}.json",
                optional: true,
                reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        ConfigureBuilder(builder, configuration.Build());

        var app = builder.Build();
        return SetupApplication(app);
    }

    private static void ConfigureBuilder(
        WebApplicationBuilder builder,
        IConfigurationRoot configuration)
    {
        // Configure logging to use the CDP Platform standards.
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHealthChecks();
        builder.Host.UseSerilog(CdpLogging.Configuration);
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<ApiExceptionHandler>();
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
            options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
        });

        // Add custom services
        builder.Services
            .AddPostgresDatabase(configuration)
            .AddRepositories(configuration)
            .AddServices(configuration)
            .AddValidators();
    }

    [ExcludeFromCodeCoverage]
    private static WebApplication SetupApplication(WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseExceptionHandler();
        app.UseRouting();
        app.UsePublicEndpoints();

        return app;
    }
}
