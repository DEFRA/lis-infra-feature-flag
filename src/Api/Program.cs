// <copyright file="Program.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Lis.Infra.FeatureFlag.Api.Endpoints.Health;
using Lis.Infra.FeatureFlag.Api.Endpoints.Public;
using Lis.Infra.FeatureFlag.Api.Exceptions;
using Lis.Infra.FeatureFlag.Api.Extensions;
using Lis.Infra.FeatureFlag.Api.Utils.Logging;
using Lis.Infra.FeatureFlag.Database;
using Lis.Infra.FeatureFlag.Models;
using Lis.Infra.FeatureFlag.Repositories;
using Lis.Infra.FeatureFlag.Services;
using Serilog;

[ExcludeFromCodeCoverage]
public static class Program
{
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

        builder.Services
            .AddPostgresDatabase(configuration)
            .AddRepositories(configuration)
            .AddRequests(configuration)
            .AddValidators()
            .AddServices(configuration);
    }

    [ExcludeFromCodeCoverage]
    private static WebApplication SetupApplication(WebApplication app)
    {
        app.UseSerilogRequestLogging();
        app.UseExceptionHandler();
        app.UseRouting();
        app.UseRequests();
        app.UseHealthEndpoints();
        app.UsePublicEndpoints();

        return app;
    }
}
