// <copyright file="FeatureFlagStatusesExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database.Extensions;

using Lis.Infra.FeatureFlag.Database.Domain;

public static class FeatureFlagStatusesExtensions
{
    extension(FeatureFlagStatuses? featureFlagStatus)
    {
        public bool IsFlagActive()
        {
            var currentDateTime = DateTime.UtcNow;

            if (featureFlagStatus is null)
            {
                return false;
            }

            switch (featureFlagStatus.ActivationType)
            {
                case ActivationType.Manual:
                    return featureFlagStatus.ManualEnabled ?? false;

                case ActivationType.Scheduled:
                {
                    var expireAt = featureFlagStatus.ExpireAt ?? DateTime.MaxValue.ToUniversalTime();
                    var activateAfter = featureFlagStatus.ActivateAfter ?? DateTime.MinValue.ToUniversalTime();

                    return currentDateTime >= activateAfter && currentDateTime < expireAt;
                }

                default:
                    return false;
            }
        }
    }
}
