// <copyright file="OpenApiMetadata.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Endpoints.Profiles;

public static class OpenApiMetadata
{
    // GetUserProfileById endpoint
    public static class GetUserProfileByIdRoute
    {
        public const string Name = "GetUserProfileById";
        public const string Summary = "Get a user profile by the associated user account id";

        public const string Description =
            "Retrieves a user profile containing user details, cph assignments and cph delegations given the id of the associated user account";
    }
}
