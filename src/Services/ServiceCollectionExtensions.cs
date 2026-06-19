// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServices(IConfigurationRoot config)
        {
            services.AddTransient<IFeatureService, FeatureService>();

            return services;
        }
    }
}
