// <copyright file="FeatureFlags.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database.Entities;

public class FeatureFlags
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public Guid? GroupId { get; set; }

    public FeatureGroups? Group { get; set; }

    public ICollection<FeatureFlagStatuses> FeatureFlagStatuses { get; set; } = new List<FeatureFlagStatuses>();
}
