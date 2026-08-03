// <copyright file="RequestHeaderNamesTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Tests.Middleware.Headers;

using Lis.Infra.FeatureFlag.Api.Middleware.Headers;

public class RequestHeaderNamesTests
{
    [Fact]
    public void HeaderNames_Should_Have_Correct_Values()
    {
        RequestHeaderNames.CorrelationId.ShouldBe("x-cdp-request-id");
        RequestHeaderNames.ApiKey.ShouldBe("x-api-key");

        RequestHeaderNames.ProductName.ShouldBe("product-name");
        RequestHeaderNames.EnvironmentName.ShouldBe("environment-name");
    }
}
