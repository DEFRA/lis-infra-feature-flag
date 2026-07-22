// <copyright file="FeatureFlagStatusResult.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Models.Responses;

using Lis.Infra.FeatureFlag.Models.Responses.Common;

public class FeatureFlagStatusResult : FeatureFlagStatus
{
    public bool Success { get; set; }
}
