// <copyright file="IFeatureService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Services;

using Lis.Infra.FeatureFlag.Models.Requests;
using Lis.Infra.FeatureFlag.Models.Responses;

public interface IFeatureService
{
    Task<EvaluationResult> EvaluateFeatureFlagTask(
        EvaluationRequest request,
        CancellationToken cancellationToken = default);
}
