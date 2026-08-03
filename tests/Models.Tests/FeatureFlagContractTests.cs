// <copyright file="FeatureFlagContractTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Models.Tests;

using Lis.Infra.FeatureFlag.Models.Requests.FeatureFlags;
using Lis.Infra.FeatureFlag.Models.Responses.FeatureFlags;
using Lis.Infra.FeatureFlag.Models.Responses.FeatureFlags.Common;

public class FeatureFlagContractTests
{
    [Fact]
    public void GetFeatureFlagStatus_ShouldStoreProvidedValues()
    {
        var request = new GetFeatureFlagStatus
        {
            GroupName = "Payments", FlagName = "NewUi", EnvironmentName = "Prod", ProductName = "LIS",
        };

        request.GroupName.ShouldBe("Payments");
        request.FlagName.ShouldBe("NewUi");
        request.EnvironmentName.ShouldBe("Prod");
        request.ProductName.ShouldBe("LIS");
    }

    [Fact]
    public void FeatureFlagStatusResult_ShouldStoreProvidedValues()
    {
        var result = new FeatureFlagStatusResult { FlagName = "Test", FlagEnabled = true, Success = true };

        result.FlagName.ShouldBe("Test");
        result.FlagEnabled.ShouldBeTrue();
        result.Success.ShouldBeTrue();
    }

    [Fact]
    public void GetFeatureFlagGroupStatus_ShouldStoreProvidedValues()
    {
        var request = new GetFeatureFlagGroupStatus
        {
            GroupName = "Payments", EnvironmentName = "Prod", ProductName = "LIS",
        };

        request.GroupName.ShouldBe("Payments");
        request.EnvironmentName.ShouldBe("Prod");
        request.ProductName.ShouldBe("LIS");
    }

    [Fact]
    public void FeatureFlagGroupStatusResult_ShouldStoreProvidedValues()
    {
        var request = new FeatureFlagGroupStatusResult()
        {
            GroupName = "Payments",
            GroupEnabled = true,
            Success = true,
            Features =
            [
                new FeatureFlagStatus() { FlagName = "flag-1", FlagEnabled = true, },
                new FeatureFlagStatus() { FlagName = "flag-2", FlagEnabled = false, }
            ],
        };

        request.GroupName.ShouldBe("Payments");
        request.GroupEnabled.ShouldBeTrue();
        request.Success.ShouldBeTrue();
        request.Features.Count.ShouldBe(2);

        request.Features[0].FlagName.ShouldBe("flag-1");
        request.Features[0].FlagEnabled.ShouldBeTrue();

        request.Features[1].FlagName.ShouldBe("flag-2");
        request.Features[1].FlagEnabled.ShouldBeFalse();
    }
}
