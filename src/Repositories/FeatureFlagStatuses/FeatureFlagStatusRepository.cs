// <copyright file="FeatureFlagStatuesRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Repositories.FeatureFlagStatuses;

using System.Linq.Expressions;
using Lis.Infra.FeatureFlag.Database;
using Lis.Infra.FeatureFlag.Database.Entities;
using Microsoft.Extensions.Logging;

public partial class FeatureFlagStatusRepository(
    PostgresDbContext context,
    ReadOnlyPostgresDbContext readOnlyContext,
    ILogger<FeatureFlagStatusRepository> logger)
    : IFeatureFlagStatusRepository
{
    public async Task<List<FeatureFlagStatuses>?> GetFlagsTask(
        Expression<Func<FeatureFlagStatuses, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        LogGettingSingleCountyParishHolding();

        var result = await readOnlyContext.FeatureFlagStatuses
            .AsSplitQuery()
            .Where(predicate)
            .Include(p => p.Environment)
            .Include(p => p.Flag)
            .Include(p => p.Group)
            .ToListAsync(cancellationToken);

        return result;
    }
}
