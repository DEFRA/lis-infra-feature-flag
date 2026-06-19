// <copyright file="FeatureFlagStatusesExtensionsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database.PostgreSql.Tests;

using Lis.Infra.FeatureFlag.Database.Domain;
using Lis.Infra.FeatureFlag.Database.Entities;
using Lis.Infra.FeatureFlag.Database.Extensions;

public class FeatureFlagStatusesExtensionsTests
{
    [Fact]
    public void IsFlagActive_ShouldReturnFalse_WhenStatusIsNull()
    {
        FeatureFlagStatuses? status = null;

        status.IsFlagActive().ShouldBeFalse();
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    [InlineData(null, false)]
    public void IsFlagActive_ShouldReturnExpectedValue_ForManualFlags(bool? manualEnabled, bool expected)
    {
        var status = new FeatureFlagStatuses
        {
            ActivationType = ActivationType.Manual,
            ManualEnabled = manualEnabled,
            UpdatedBy = "tester",
            Group = new FeatureGroups(),
        };

        status.IsFlagActive().ShouldBe(expected);
    }

    [Fact]
    public void IsFlagActive_ShouldReturnTrue_WhenScheduledWindowIsActive()
    {
        var status = new FeatureFlagStatuses
        {
            ActivationType = ActivationType.Scheduled,
            ActivateAfter = DateTime.UtcNow.AddMinutes(-5),
            ExpireAt = DateTime.UtcNow.AddMinutes(5),
            UpdatedBy = "tester",
            Group = new FeatureGroups(),
        };

        status.IsFlagActive().ShouldBeTrue();
    }

    [Fact]
    public void IsFlagActive_ShouldReturnFalse_WhenScheduledWindowHasExpired()
    {
        var status = new FeatureFlagStatuses
        {
            ActivationType = ActivationType.Scheduled,
            ActivateAfter = DateTime.UtcNow.AddMinutes(-10),
            ExpireAt = DateTime.UtcNow.AddMinutes(-1),
            UpdatedBy = "tester",
            Group = new FeatureGroups(),
        };

        status.IsFlagActive().ShouldBeFalse();
    }
}
