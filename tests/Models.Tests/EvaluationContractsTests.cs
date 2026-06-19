// <copyright file="EvaluationContractsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Models.Tests;

using Lis.Infra.FeatureFlag.Models.Requests;
using Lis.Infra.FeatureFlag.Models.Responses;

public class EvaluationContractsTests
{
    [Fact]
    public void EvaluationRequest_ShouldStoreProvidedValues()
    {
        var request = new EvaluationRequest
        {
            Group = "Payments",
            Flag = "NewUi",
            Environment = "Prod",
        };

        request.Group.ShouldBe("Payments");
        request.Flag.ShouldBe("NewUi");
        request.Environment.ShouldBe("Prod");
    }

    [Fact]
    public void EvaluationResult_ShouldStoreProvidedValues()
    {
        var result = new EvaluationResult
        {
            Success = true,
            IsEnabled = true,
        };

        result.Success.ShouldBeTrue();
        result.IsEnabled.ShouldBeTrue();
    }
}
