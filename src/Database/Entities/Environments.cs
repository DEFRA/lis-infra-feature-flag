// <copyright file="Environments.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database.Entities;

public class Environments
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public ICollection<FeatureFlagStatuses> FeatureFlagStatuses { get; set; } = new List<FeatureFlagStatuses>();
}
