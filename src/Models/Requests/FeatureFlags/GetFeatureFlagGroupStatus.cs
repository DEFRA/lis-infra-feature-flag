// <copyright file="GetFeatureFlagGroupStatus.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Models.Requests.FeatureFlags;

using Lis.Infra.FeatureFlag.Models.Requests.FeatureFlags.Base;

public class GetFeatureFlagGroupStatus : OperationByProductAndEnvironment
{
    public required string GroupName { get; set; }
}
