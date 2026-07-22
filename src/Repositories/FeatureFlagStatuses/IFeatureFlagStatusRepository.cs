// <copyright file="IFeatureFlagStatusRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Repositories.FeatureFlagStatuses;

using Defra.Livestock.Sdk.Api.Strategies.Abstractions.Repositories;
using Lis.Infra.FeatureFlag.Database.Entities;

public interface IFeatureFlagStatusRepository : IRepoListable<FeatureFlagStatuses>;
