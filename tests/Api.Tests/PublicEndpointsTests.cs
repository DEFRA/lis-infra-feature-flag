// <copyright file="PublicEndpointsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Tests;

using System.Reflection;
using Lis.Infra.FeatureFlag.Api.Endpoints.Public;
using Lis.Infra.FeatureFlag.Models.Requests;
using Lis.Infra.FeatureFlag.Models.Responses;
using Lis.Infra.FeatureFlag.Models.Responses.Common;
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

        endpoints.Count.ShouldBe(2);
        routePatterns.ShouldBe(
        [
            "evaluate/{environment:regex(^(?i)(DEV|TEST|EXT-TEST|PROD)$)}/{group}",
            "evaluate/{environment:regex(^(?i)(DEV|TEST|EXT-TEST|PROD)$)}/{group}/{flag}",
        ]);
        endpoints.All(endpoint => endpoint.Metadata.GetMetadata<IEndpointNameMetadata>()?.EndpointName ==
                                  OpenApiMetadata.GetFeatureFlagGroupStatusRoute.Name)
            .ShouldBeTrue();
        endpoints.All(endpoint => endpoint.Metadata.GetMetadata<IEndpointSummaryMetadata>()?.Summary ==
                                  OpenApiMetadata.GetFeatureFlagGroupStatusRoute.Summary)
            .ShouldBeTrue();
        endpoints.All(endpoint => endpoint.Metadata.GetMetadata<IEndpointDescriptionMetadata>()?.Description ==
                                  OpenApiMetadata.GetFeatureFlagGroupStatusRoute.Description)
            .ShouldBeTrue();
        endpoints.All(endpoint =>
                endpoint.Metadata.GetMetadata<HttpMethodMetadata>()!.HttpMethods.Single() == HttpMethods.Get)
            .ShouldBeTrue();
    }

    [Fact]
    public async Task EvaluatedFeatureFlag_ShouldReturnOkResultFromService()
    {
        var service = Substitute.For<IFeatureFlagService>();
        service.GetFeatureFlagStatus(Arg.Any<GetFeatureFlagStatus>(), Arg.Any<CancellationToken>())
            .Returns(new FeatureFlagStatus { FlagEnabled = true });
        var method = typeof(PublicEndpoints).GetMethod("EvaluateFeatureFlagStatusByGroupAndFlag",
            BindingFlags.NonPublic | BindingFlags.Static);
        var request = new GetFeatureFlagStatus { GroupName = "Payments", FlagName = "NewUi", EnvironmentName = "Prod", };

        method.ShouldNotBeNull();

        var task = (Task<IResult>)method!.Invoke(null, [request, service, TestContext.Current.CancellationToken])!;
        var result = await task;
        var okResult = result.ShouldBeOfType<Ok<FeatureFlagStatus>>();

        okResult.Value.ShouldBe(new FeatureFlagStatus { FlagEnabled = true });
    }
}
