// <copyright file="PublicEndpoints.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Endpoints.Public;

using System.Net.Mime;
using Defra.Identity.Api.Endpoints.Profiles;
using Lis.Infra.FeatureFlag.Models.Requests;
using Lis.Infra.FeatureFlag.Models.Responses;
using Lis.Infra.FeatureFlag.Services;
using Microsoft.AspNetCore.Mvc;

public static class PublicEndpoints
{
    public static void UsePublicEndpoints(this IEndpointRouteBuilder app)
    {
        const string environmentConstraint = "{environment:regex(^(?i)(DEV|TEST|EXT-TEST|PROD)$)}";

        app.MapGet(RouteNames.Evaluate + $"/{{{environmentConstraint}}}/{{group}}", EvaluatedFeatureFlag)
            .WithName(OpenApiMetadata.GetUserProfileByIdRoute.Name)
            .WithTags(nameof(RouteNames.Evaluate))
            .WithSummary(OpenApiMetadata.GetUserProfileByIdRoute.Summary)
            .WithDescription(OpenApiMetadata.GetUserProfileByIdRoute.Description)
            .Produces<EvaluationResult>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status404NotFound);

        app.MapGet(RouteNames.Evaluate + $"/{{{environmentConstraint}}}/{{group}}/{{flag}}", EvaluatedFeatureFlag)
            .WithName(OpenApiMetadata.GetUserProfileByIdRoute.Name)
            .WithTags(nameof(RouteNames.Evaluate))
            .WithSummary(OpenApiMetadata.GetUserProfileByIdRoute.Summary)
            .WithDescription(OpenApiMetadata.GetUserProfileByIdRoute.Description)
            .Produces<EvaluationResult>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> EvaluatedFeatureFlag(
        [AsParameters] EvaluationRequest request,
        [FromServices] IFeatureService service,
        CancellationToken cancellationToken = default)
    {
        var result = await service.EvaluateFeatureFlagTask(
            request,
            cancellationToken);

        return Results.Ok(result);
    }
}
