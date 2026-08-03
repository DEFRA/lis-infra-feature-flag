// <copyright file="FeatureFlagStatusResult.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Models.Responses.FeatureFlags;

using Lis.Infra.FeatureFlag.Models.Responses.FeatureFlags.Common;

public class FeatureFlagStatusResult : FeatureFlagStatus
{
    public required bool Success { get; init; }
}
