// <copyright file="OpenApiMetadata.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Endpoints.Public;

public static class OpenApiMetadata
{
    public static class GetFeatureFlagGroupStatusRoute
    {
        public const string Name = "GetFeatureFlagGroupStatus";
        public const string Summary = "Get feature flag group status";

        public const string Description =
            "Retrieves the feature flag statuses for a given group";
    }

    public static class GetFeatureFlagStatusRoute
    {
        public const string Name = "GetFeatureFlagStatus";
        public const string Summary = "Get feature flag status";

        public const string Description =
            "Retrieves the feature flag status for a given flag within a group";
    }
}
