// <copyright file="FeatureGroupAndFlagStatuses.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Services.Models;

using Lis.Infra.FeatureFlag.Database.Entities;
using Lis.Infra.FeatureFlag.Database.Extensions;

public class FeatureGroupAndFlagStatuses
{
    public FeatureFlagStatuses? EnvironmentAgnosticGroupStatus { get; init; }

    public FeatureFlagStatuses? EnvironmentSpecificGroupStatus { get; init; }

    public List<FeatureFlagStatuses> FeatureFlagStatuses { get; init; } = [];

    public bool IsGroupEnabled => (EnvironmentSpecificGroupStatus ?? EnvironmentAgnosticGroupStatus).IsFlagActive();

    public bool HasGroupStatus => EnvironmentSpecificGroupStatus != null || EnvironmentAgnosticGroupStatus != null;

    public List<string?> GetUniqueFeatureFlagNames() =>
        FeatureFlagStatuses.Select(status => status.Flag?.Name).Distinct().ToList();

    public bool GetHasFlagWithName(string flagName) =>
        FeatureFlagStatuses.Any(status => status.Flag?.Name == flagName);
}
