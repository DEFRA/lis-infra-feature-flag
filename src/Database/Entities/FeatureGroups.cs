// <copyright file="FeatureGroups.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database.Entities;

public class FeatureGroups
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public ICollection<FeatureFlags> FeatureFlags { get; set; } = new List<FeatureFlags>();

    public ICollection<FeatureFlagStatuses> FeatureFlagStatuses { get; set; } = new List<FeatureFlagStatuses>();
}
