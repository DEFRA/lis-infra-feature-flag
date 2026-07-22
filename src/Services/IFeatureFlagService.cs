// <copyright file="IFeatureFlagService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Services;

using Lis.Infra.FeatureFlag.Models.Requests;
using Lis.Infra.FeatureFlag.Models.Responses;
using Lis.Infra.FeatureFlag.Models.Responses.Common;

public interface IFeatureFlagService
{
    Task<FeatureFlagGroupStatusResult> GetFeatureFlagGroupStatus(
        GetFeatureFlagGroupStatus request,
        CancellationToken cancellationToken = default);

    Task<FeatureFlagStatusResult> GetFeatureFlagStatus(
        GetFeatureFlagStatus request,
        CancellationToken cancellationToken = default);
}
