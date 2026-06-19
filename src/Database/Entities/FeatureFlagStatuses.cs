// <copyright file="FeatureFlagStatuses.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database.Entities;

using Lis.Infra.FeatureFlag.Database.Domain;

public class FeatureFlagStatuses
{
    public Guid Id { get; set; }

    public Guid GroupId { get; set; }

    public Guid? FlagId { get; set; }

    public Guid? EnvironmentId { get; set; }

    public ActivationType ActivationType { get; set; }

    public bool? ManualEnabled { get; set; }

    public DateTime? ActivateAfter { get; set; }

    public DateTime? ExpireAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string UpdatedBy { get; set; } = null!;

    public FeatureGroups Group { get; set; } = null!;

    public FeatureFlags? Flag { get; set; }

    public Environments? Environment { get; set; }
}
