// <copyright file="FeatureFlagStatus.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Models.Responses.Common;

public class FeatureFlagStatus()
{
    public required string FlagName { get; set; }

    public bool FlagEnabled { get; init; }
}
