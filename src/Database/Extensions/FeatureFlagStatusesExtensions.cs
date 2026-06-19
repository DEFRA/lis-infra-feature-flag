// <copyright file="FeatureFlagStatusesExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database.Extensions;

using System.Diagnostics;
using Lis.Infra.FeatureFlag.Database.Domain;

public static class FeatureFlagStatusesExtensions
{
    extension(FeatureFlagStatuses? flag)
    {
        public bool IsFlagActive()
        {
            var now = DateTime.UtcNow;

            if (flag is null)
            {
                return false;
            }

            switch (flag.ActivationType)
            {
                case ActivationType.Manual:
                    return flag.ManualEnabled ?? false;

                case ActivationType.Scheduled:
                {
                    var expireAt = flag.ExpireAt ?? DateTime.MaxValue.ToUniversalTime();
                    Debug.Assert(flag.ActivateAfter != null, "flag.ActivateAfter != null");
                    return now >= flag.ActivateAfter.Value && now < expireAt;
                }

                default:
                    return false;
            }
        }
    }
}
