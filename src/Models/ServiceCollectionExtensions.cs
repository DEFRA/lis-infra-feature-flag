// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Models;

using FluentValidation;
using Lis.Infra.FeatureFlag.Models.Requests.FeatureFlags;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddValidators()
        {
            services.AddValidatorsFromAssemblyContaining<GetFeatureFlagStatus>();

            return services;
        }
    }
}
