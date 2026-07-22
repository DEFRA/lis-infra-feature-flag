// <copyright file="EvaluationContractsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Models.Tests;

using Lis.Infra.FeatureFlag.Models.Requests;
using Lis.Infra.FeatureFlag.Models.Responses;
using Lis.Infra.FeatureFlag.Models.Responses.Common;

public class EvaluationContractsTests
{
    [Fact]
    public void EvaluationRequest_ShouldStoreProvidedValues()
    {
        var request = new GetFeatureFlagStatus
        {
            GroupName = "Payments",
            FlagName = "NewUi",
            EnvironmentName = "Prod",
        };

        request.GroupName.ShouldBe("Payments");
        request.FlagName.ShouldBe("NewUi");
        request.EnvironmentName.ShouldBe("Prod");
    }

    [Fact]
    public void EvaluationResult_ShouldStoreProvidedValues()
    {
        var result = new FeatureFlagStatusResult
        {
            FlagName = "Test",
            FlagEnabled = true,
            Success = false,
        };

        result.FlagName.ShouldBe("Test");
        result.FlagEnabled.ShouldBeTrue();
        result.Success.ShouldBeFalse();
    }
}
