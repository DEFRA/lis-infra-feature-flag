// <copyright file="FeatureFlagGroupStatusResult.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Models.Responses;

public class FeatureFlagGroupStatusResult
{
    public required string GroupName { get; set; }

    public bool GroupEnabled { get; set; }

    public List<FeatureFlagStatusResult> Features { get; set; } = [];
}
