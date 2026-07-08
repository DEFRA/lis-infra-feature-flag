// <copyright file="Products.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database.Entities;

public class Products
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public ICollection<FeatureGroups> FeatureGroups { get; set; } = new List<FeatureGroups>();

    public ICollection<FeatureFlags> FeatureFlags { get; set; } = new List<FeatureFlags>();

    public ICollection<FeatureFlagStatuses> FeatureFlagStatuses { get; set; } = new List<FeatureFlagStatuses>();
}
