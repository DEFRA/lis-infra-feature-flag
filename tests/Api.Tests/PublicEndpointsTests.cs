// <copyright file="PublicEndpointsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Tests;

using System.Reflection;
using Defra.Identity.Api.Endpoints.Profiles;
using Lis.Infra.FeatureFlag.Api.Endpoints.Public;
using Lis.Infra.FeatureFlag.Models.Requests;
using Lis.Infra.FeatureFlag.Models.Responses;
using Lis.Infra.FeatureFlag.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using NSubstitute;

public class PublicEndpointsTests
{
    [Fact]
    public void UsePublicEndpoints_ShouldMapExpectedEvaluateRoutes()
    {
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();

        app.UsePublicEndpoints();

        var endpoints = ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(dataSource => dataSource.Endpoints)
            .OfType<RouteEndpoint>()
            .ToList();
        var routePatterns = endpoints
            .Select(endpoint => endpoint.RoutePattern.RawText)
            .OrderBy(pattern => pattern)
            .ToList();

        endpoints.Count.ShouldBe(3);
        routePatterns.ShouldBe(
        [
            "evaluate/{group}",
            "evaluate/{group}/{flag}",
            "evaluate/{group}/{flag}/{environment}",
        ]);
        endpoints.All(endpoint => endpoint.Metadata.GetMetadata<IEndpointNameMetadata>()?.EndpointName == OpenApiMetadata.GetUserProfileByIdRoute.Name)
            .ShouldBeTrue();
        endpoints.All(endpoint => endpoint.Metadata.GetMetadata<IEndpointSummaryMetadata>()?.Summary == OpenApiMetadata.GetUserProfileByIdRoute.Summary)
            .ShouldBeTrue();
        endpoints.All(endpoint => endpoint.Metadata.GetMetadata<IEndpointDescriptionMetadata>()?.Description == OpenApiMetadata.GetUserProfileByIdRoute.Description)
            .ShouldBeTrue();
        endpoints.All(endpoint => endpoint.Metadata.GetMetadata<HttpMethodMetadata>()!.HttpMethods.Single() == HttpMethods.Get)
            .ShouldBeTrue();
    }

    [Fact]
    public async Task EvaluatedFeatureFlag_ShouldReturnOkResultFromService()
    {
        var service = Substitute.For<IFeatureService>();
        service.EvaluateFeatureFlagTask(Arg.Any<EvaluationRequest>(), Arg.Any<CancellationToken>())
            .Returns(new EvaluationResult { Success = true, IsEnabled = true });
        var method = typeof(PublicEndpoints).GetMethod("EvaluatedFeatureFlag", BindingFlags.NonPublic | BindingFlags.Static);
        var request = new EvaluationRequest
        {
            Group = "Payments",
            Flag = "NewUi",
            Environment = "Prod",
        };

        method.ShouldNotBeNull();

        var task = (Task<IResult>)method!.Invoke(null, [request, service, TestContext.Current.CancellationToken])!;
        var result = await task;
        var okResult = result.ShouldBeOfType<Ok<EvaluationResult>>();

        okResult.Value.ShouldBe(new EvaluationResult { Success = true, IsEnabled = true });
    }
}
