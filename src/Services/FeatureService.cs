// <copyright file="FeatureService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Services;

using Lis.Infra.FeatureFlag.Database.Extensions;
using Lis.Infra.FeatureFlag.Models.Requests;
using Lis.Infra.FeatureFlag.Models.Responses;
using Lis.Infra.FeatureFlag.Repositories.FeatureFlagStatuses;

public class FeatureService(
    IFeatureFlagStatusRepository featureFlagStatusRepository)
    : IFeatureService
{
    public async Task<EvaluationResult> EvaluateFeatureFlagTask(
        EvaluationRequest request,
        CancellationToken cancellationToken = default)
    {
        var flags = await featureFlagStatusRepository
            .GetFlagsTask(
                x =>
                    x.Group.Name.Equals(request.Group, StringComparison.InvariantCultureIgnoreCase) &&
                    (x.Flag == null || x.Flag.Name.Equals(request.Flag, StringComparison.InvariantCultureIgnoreCase)),
                cancellationToken);

        if (flags == null)
        {
            return new EvaluationResult { Success = true, IsEnabled = false, };
        }

        var group = flags.SingleOrDefault(x =>
            x.Flag == null &&
            (x.Environment == null ||
             x.Environment.Name.Equals(request.Environment, StringComparison.InvariantCultureIgnoreCase)));

        var flag = flags.SingleOrDefault(x =>
            x.Flag != null && x.Flag.Name.Equals(request.Flag, StringComparison.InvariantCultureIgnoreCase) &&
            (x.Environment == null ||
             x.Environment.Name.Equals(request.Environment, StringComparison.InvariantCultureIgnoreCase)));

        if (!group.IsFlagActive())
        {
            return new EvaluationResult() { Success = true, IsEnabled = false, };
        }

        return new EvaluationResult()
        {
            Success = true,
            IsEnabled = flag.IsFlagActive(),
        };
    }
}
