// <copyright file="FeatureServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Services.Tests;

using System.Linq.Expressions;
using Lis.Infra.FeatureFlag.Database.Domain;
using Lis.Infra.FeatureFlag.Database.Entities;
using Lis.Infra.FeatureFlag.Models.Requests;
using Lis.Infra.FeatureFlag.Repositories.FeatureFlagStatuses;
using NSubstitute;

public class FeatureServiceTests
{
    [Fact]
    public async Task EvaluateFeatureFlagTask_ShouldReturnDisabled_WhenRepositoryReturnsNull()
    {
        var repository = Substitute.For<IFeatureFlagStatusRepository>();
        repository.GetFlagsTask(Arg.Any<Expression<Func<FeatureFlagStatuses, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((List<FeatureFlagStatuses>?)null);

        var sut = new FeatureService(repository);

        var result = await sut.EvaluateFeatureFlagTask(CreateRequest(), TestContext.Current.CancellationToken);

        result.Success.ShouldBeTrue();
        result.IsEnabled.ShouldBeFalse();
    }

    [Fact]
    public async Task EvaluateFeatureFlagTask_ShouldReturnDisabled_WhenGroupIsInactive()
    {
        var repository = Substitute.For<IFeatureFlagStatusRepository>();
        repository.GetFlagsTask(Arg.Any<Expression<Func<FeatureFlagStatuses, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(
            [
                CreateStatus(isGroupStatus: true, manualEnabled: false),
                CreateStatus(isGroupStatus: false, manualEnabled: true),
            ]);

        var sut = new FeatureService(repository);

        var result = await sut.EvaluateFeatureFlagTask(CreateRequest(), TestContext.Current.CancellationToken);

        result.Success.ShouldBeTrue();
        result.IsEnabled.ShouldBeFalse();
    }

    [Fact]
    public async Task EvaluateFeatureFlagTask_ShouldReturnEnabled_WhenGroupAndFlagAreActive()
    {
        var repository = Substitute.For<IFeatureFlagStatusRepository>();
        repository.GetFlagsTask(Arg.Any<Expression<Func<FeatureFlagStatuses, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(
            [
                CreateStatus(isGroupStatus: true, manualEnabled: true),
                CreateStatus(isGroupStatus: false, manualEnabled: true),
            ]);

        var sut = new FeatureService(repository);

        var result = await sut.EvaluateFeatureFlagTask(CreateRequest(), TestContext.Current.CancellationToken);

        result.Success.ShouldBeTrue();
        result.IsEnabled.ShouldBeTrue();
    }

    [Fact]
    public async Task EvaluateFeatureFlagTask_ShouldReturnDisabled_WhenFlagStatusIsMissing()
    {
        var repository = Substitute.For<IFeatureFlagStatusRepository>();
        repository.GetFlagsTask(Arg.Any<Expression<Func<FeatureFlagStatuses, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(
            [
                CreateStatus(isGroupStatus: true, manualEnabled: true),
            ]);

        var sut = new FeatureService(repository);

        var result = await sut.EvaluateFeatureFlagTask(CreateRequest(), TestContext.Current.CancellationToken);

        result.Success.ShouldBeTrue();
        result.IsEnabled.ShouldBeFalse();
    }

    [Fact]
    public async Task EvaluateFeatureFlagTask_ShouldUseScheduledStatus_WhenWindowIsActive()
    {
        var repository = Substitute.For<IFeatureFlagStatusRepository>();
        repository.GetFlagsTask(Arg.Any<Expression<Func<FeatureFlagStatuses, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(
            [
                CreateStatus(
                    isGroupStatus: true,
                    activationType: ActivationType.Scheduled,
                    activateAfter: DateTime.UtcNow.AddMinutes(-5),
                    expireAt: DateTime.UtcNow.AddMinutes(5)),
                CreateStatus(
                    isGroupStatus: false,
                    activationType: ActivationType.Scheduled,
                    activateAfter: DateTime.UtcNow.AddMinutes(-5),
                    expireAt: DateTime.UtcNow.AddMinutes(5)),
            ]);

        var sut = new FeatureService(repository);

        var result = await sut.EvaluateFeatureFlagTask(CreateRequest(), TestContext.Current.CancellationToken);

        result.Success.ShouldBeTrue();
        result.IsEnabled.ShouldBeTrue();
    }

    private static EvaluationRequest CreateRequest()
    {
        return new EvaluationRequest
        {
            Group = "Payments",
            Flag = "NewUi",
            Environment = "Prod",
        };
    }

    private static FeatureFlagStatuses CreateStatus(
        bool isGroupStatus,
        ActivationType activationType = ActivationType.Manual,
        bool? manualEnabled = true,
        DateTime? activateAfter = null,
        DateTime? expireAt = null)
    {
        return new FeatureFlagStatuses
        {
            ActivationType = activationType,
            ManualEnabled = manualEnabled,
            ActivateAfter = activateAfter,
            ExpireAt = expireAt,
            Group = new FeatureGroups
            {
                Name = "Payments",
            },
            Flag = isGroupStatus
                ? null
                : new FeatureFlags
                {
                    Name = "NewUi",
                },
            Environment = new Environments
            {
                Name = "Prod",
            },
            UpdatedBy = "tester",
        };
    }
}
