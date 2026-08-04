// <copyright file="HealthEndpointsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Tests.Endpoints.Health;

using Lis.Infra.FeatureFlag.Api.Endpoints.Health;
using Lis.Infra.FeatureFlag.Models.Responses.Health;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

public class HealthEndpointsTests
{
    [Fact]
    public async Task Get_ReturnsOk()
    {
        // Act
        var result = await (Task<IResult>)typeof(HealthEndpoints)
            .GetMethod(
                "CalculateHealthRoute",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [])!;

        // Assert
        result.ShouldBeOfType<Ok<HealthStatus>>();

        ((Ok<HealthStatus>)result).Value!.Status.ShouldBe("ok");
    }
}
