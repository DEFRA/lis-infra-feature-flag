// <copyright file="PublicEndpointsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Tests.Endpoints.FeatureFlags;

using System.Reflection;
using Lis.Infra.FeatureFlag.Api.Endpoints.Public;
using Lis.Infra.FeatureFlag.Models.Requests.FeatureFlags;
using Lis.Infra.FeatureFlag.Models.Responses.FeatureFlags;
using Lis.Infra.FeatureFlag.Models.Responses.FeatureFlags.Common;
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
            "evaluate/{groupName}",
            "evaluate/{groupName}/{flagName}",
        ]);

        endpoints[0].Metadata.GetMetadata<IEndpointNameMetadata>()?.EndpointName
            .ShouldBe(OpenApiMetadata.GetFeatureFlagGroupStatusRoute.Name);
        endpoints[0].Metadata.GetMetadata<IEndpointSummaryMetadata>()?.Summary
            .ShouldBe(OpenApiMetadata.GetFeatureFlagGroupStatusRoute.Summary);
        endpoints[0].Metadata.GetMetadata<IEndpointDescriptionMetadata>()?.Description
            .ShouldBe(OpenApiMetadata.GetFeatureFlagGroupStatusRoute.Description);
        endpoints[0].Metadata.GetMetadata<HttpMethodMetadata>()!.HttpMethods.Single().ShouldBe(HttpMethods.Get);

        endpoints[1].Metadata.GetMetadata<IEndpointNameMetadata>()?.EndpointName
            .ShouldBe(OpenApiMetadata.GetFeatureFlagStatusRoute.Name);
        endpoints[1].Metadata.GetMetadata<IEndpointSummaryMetadata>()?.Summary
            .ShouldBe(OpenApiMetadata.GetFeatureFlagStatusRoute.Summary);
        endpoints[1].Metadata.GetMetadata<IEndpointDescriptionMetadata>()?.Description
            .ShouldBe(OpenApiMetadata.GetFeatureFlagStatusRoute.Description);
        endpoints[1].Metadata.GetMetadata<HttpMethodMetadata>()!.HttpMethods.Single().ShouldBe(HttpMethods.Get);
    }

    [Fact]
    public async Task GetFeatureFlagStatus_ShouldReturnOkResultFromService()
    {
        var service = Substitute.For<IFeatureFlagService>();
        service.GetFeatureFlagStatus(Arg.Any<GetFeatureFlagStatus>(), Arg.Any<CancellationToken>())
            .Returns(new FeatureFlagStatusResult() { FlagName = "new-ui", FlagEnabled = true, Success = true });

        var method = typeof(PublicEndpoints).GetMethod(
            "GetFeatureFlagStatusRoute",
            BindingFlags.NonPublic | BindingFlags.Static);

        var request =
            new GetFeatureFlagStatus { GroupName = "payments", FlagName = "new-ui", EnvironmentName = "prod", };

        method.ShouldNotBeNull();

        var task = (Task<IResult>)method.Invoke(null, [request, service, TestContext.Current.CancellationToken])!;
        var result = await task;
        var okResult = result.ShouldBeOfType<Ok<FeatureFlagStatusResult>>();

        okResult.Value.ShouldNotBeNull();
        okResult.Value.FlagName.ShouldBe("new-ui");
        okResult.Value.FlagEnabled.ShouldBeTrue();
        okResult.Value.Success.ShouldBeTrue();
    }

    [Fact]
    public async Task GetFeatureFlagGroupStatus_ShouldReturnOkResultFromService()
    {
        var service = Substitute.For<IFeatureFlagService>();
        service.GetFeatureFlagGroupStatus(Arg.Any<GetFeatureFlagGroupStatus>(), Arg.Any<CancellationToken>())
            .Returns(new FeatureFlagGroupStatusResult()
            {
                GroupName = "payments",
                GroupEnabled = true,
                Success = true,
                Features =
                [
                    new FeatureFlagStatus() { FlagName = "new-ui-1", FlagEnabled = true },
                    new FeatureFlagStatus() { FlagName = "new-ui-2", FlagEnabled = false }
                ],
            });

        var method = typeof(PublicEndpoints).GetMethod(
            "GetFeatureFlagGroupStatusRoute",
            BindingFlags.NonPublic | BindingFlags.Static);

        var request =
            new GetFeatureFlagGroupStatus() { GroupName = "payments", EnvironmentName = "prod", ProductName = "LIS" };

        method.ShouldNotBeNull();

        var task = (Task<IResult>)method.Invoke(null, [request, service, TestContext.Current.CancellationToken])!;
        var result = await task;
        var okResult = result.ShouldBeOfType<Ok<FeatureFlagGroupStatusResult>>();

        okResult.Value.ShouldNotBeNull();
        okResult.Value.GroupName.ShouldBe("payments");
        okResult.Value.GroupEnabled.ShouldBeTrue();
        okResult.Value.Success.ShouldBeTrue();

        okResult.Value.Features.ShouldNotBeNull();
        okResult.Value.Features.Count.ShouldBe(2);
        okResult.Value.Features[0].FlagName.ShouldBe("new-ui-1");
        okResult.Value.Features[0].FlagEnabled.ShouldBeTrue();
        okResult.Value.Features[1].FlagName.ShouldBe("new-ui-2");
        okResult.Value.Features[1].FlagEnabled.ShouldBeFalse();
    }
}
