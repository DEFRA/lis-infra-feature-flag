// <copyright file="IFeatureFlagStatusRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Repositories.FeatureFlagStatuses;

using System.Linq.Expressions;
using Lis.Infra.FeatureFlag.Database.Entities;

public interface IFeatureFlagStatusRepository
{
    Task<List<FeatureFlagStatuses>?> GetFlagsTask(
        Expression<Func<FeatureFlagStatuses, bool>> predicate,
        CancellationToken cancellationToken = default);
}
