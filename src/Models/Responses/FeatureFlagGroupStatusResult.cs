// <copyright file="FeatureFlagGroupStatusResult.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Models.Responses;

using Lis.Infra.FeatureFlag.Models.Responses.Common;

public class FeatureFlagGroupStatusResult
{
    public required string GroupName { get; set; }

    public bool GroupEnabled { get; set; }

    public List<FeatureFlagStatus> Features { get; set; } = [];

    public bool Success { get; set; }
}
