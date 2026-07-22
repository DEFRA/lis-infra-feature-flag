// <copyright file="FeatureFlagStatusResult.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Models.Responses;

public class FeatureFlagStatusResult()
{
    public required string FlagName { get; set; }

    public bool FlagEnabled { get; set; }
}
