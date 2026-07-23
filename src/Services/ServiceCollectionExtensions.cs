// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Services;

using Defra.Livestock.Sdk.Api.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServices(IConfigurationRoot config)
        {
            services.AddStrategyFramework();
            services.AddRepoStrategyFactory<FeatureFlagService>();
            services.AddTransient<IFeatureFlagService, FeatureFlagService>();

            return services;
        }
    }
}
