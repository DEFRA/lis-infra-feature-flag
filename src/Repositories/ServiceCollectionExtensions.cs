// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Repositories;

using Lis.Infra.FeatureFlag.Repositories.FeatureFlagStatuses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRepositories(IConfigurationRoot configuration)
        {
            services.AddTransient<IFeatureFlagStatusRepository, FeatureFlagStatusRepository>();

            return services;
        }
    }
}
