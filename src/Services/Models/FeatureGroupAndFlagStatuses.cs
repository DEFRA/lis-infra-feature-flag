// <copyright file="FeatureGroupAndFlagStatuses.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Services.Models;

using Lis.Infra.FeatureFlag.Database.Entities;
using Lis.Infra.FeatureFlag.Database.Extensions;

public class FeatureGroupAndFlagStatuses
{
    public FeatureFlagStatuses? EnvironmentAgnosticGroupStatus { get; set; }

    public FeatureFlagStatuses? EnvironmentSpecificGroupStatus { get; set; }

    public List<FeatureFlagStatuses> FeatureFlagStatuses { get; set; } = [];

    public bool IsGroupEnabled => (EnvironmentSpecificGroupStatus ?? EnvironmentAgnosticGroupStatus).IsFlagActive();

    public List<string?> GetUniqueFeatureFlagNames() =>
        FeatureFlagStatuses.Select(status => status.Flag?.Name).Distinct().ToList();
}
