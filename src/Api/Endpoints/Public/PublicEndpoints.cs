// <copyright file="PublicEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Endpoints.Public;

using System.Net.Mime;
using Lis.Infra.FeatureFlag.Api.Filters;
using Lis.Infra.FeatureFlag.Models.Requests.FeatureFlags;
using Lis.Infra.FeatureFlag.Models.Responses.FeatureFlags;
using Lis.Infra.FeatureFlag.Services;
using Microsoft.AspNetCore.Mvc;

public static class PublicEndpoints
{
    public static void UsePublicEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(
                RouteNames.Evaluate + $"/{{groupName}}",
                GetFeatureFlagGroupStatusRoute)
            .WithName(OpenApiMetadata.GetFeatureFlagGroupStatusRoute.Name)
            .WithTags(nameof(RouteNames.Evaluate))
            .WithSummary(OpenApiMetadata.GetFeatureFlagGroupStatusRoute.Summary)
            .WithDescription(OpenApiMetadata.GetFeatureFlagGroupStatusRoute.Description)
            .AddEndpointFilter<RequestHeaderMappingFilter<GetFeatureFlagGroupStatus>>()
            .AddEndpointFilter<ValidationFilter<GetFeatureFlagGroupStatus>>()
            .Produces<FeatureFlagGroupStatusResult>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapGet(
                RouteNames.Evaluate + $"/{{groupName}}/{{flagName}}",
                GetFeatureFlagStatusRoute)
            .WithName(OpenApiMetadata.GetFeatureFlagStatusRoute.Name)
            .WithTags(nameof(RouteNames.Evaluate))
            .WithSummary(OpenApiMetadata.GetFeatureFlagStatusRoute.Summary)
            .WithDescription(OpenApiMetadata.GetFeatureFlagStatusRoute.Description)
            .AddEndpointFilter<RequestHeaderMappingFilter<GetFeatureFlagStatus>>()
            .AddEndpointFilter<ValidationFilter<GetFeatureFlagStatus>>()
            .Produces<FeatureFlagStatusResult>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetFeatureFlagGroupStatusRoute(
        [AsParameters] GetFeatureFlagGroupStatus request,
        [FromServices] IFeatureFlagService featureFlagService,
        CancellationToken cancellationToken = default)
    {
        var result = await featureFlagService.GetFeatureFlagGroupStatus(
            request,
            cancellationToken);

        return Results.Ok(result);
    }

    private static async Task<IResult> GetFeatureFlagStatusRoute(
        [AsParameters] GetFeatureFlagStatus request,
        [FromServices] IFeatureFlagService featureFlagService,
        CancellationToken cancellationToken = default)
    {
        var result = await featureFlagService.GetFeatureFlagStatus(
            request,
            cancellationToken);

        return Results.Ok(result);
    }
}
