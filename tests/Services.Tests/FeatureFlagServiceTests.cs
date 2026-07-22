// <copyright file="FeatureServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Services.Tests;

using System.Linq.Expressions;
using Defra.Livestock.Sdk.Api.Strategies.Operations;
using Lis.Infra.FeatureFlag.Database.Domain;
using Lis.Infra.FeatureFlag.Database.Entities;
using Lis.Infra.FeatureFlag.Models.Requests;
using Lis.Infra.FeatureFlag.Repositories.FeatureFlagStatuses;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class FeatureFlagServiceTests
{
    private readonly IFeatureFlagStatusRepository repository;
    private readonly ILogger<FeatureFlagService> logger;
    private readonly FeatureFlagService sut;

    public FeatureFlagServiceTests()
    {
        repository = Substitute.For<IFeatureFlagStatusRepository>();
        logger = Substitute.For<ILogger<FeatureFlagService>>();
        sut = new FeatureFlagService(repository, new RepoStrategyFactory<FeatureFlagService>(), logger);
    }

    [Fact]
    public async Task EvaluateFeatureFlagTask_ShouldReturnDisabled_WhenGroupIsInactive()
    {
        repository.GetList(Arg.Any<Expression<Func<FeatureFlagStatuses, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(
            [
                CreateStatus(isGroupStatus: true, manualEnabled: false),
                CreateStatus(isGroupStatus: false, manualEnabled: true),
            ]);

        var result = await sut.GetFeatureFlagStatus(CreateRequest(), TestContext.Current.CancellationToken);

        result.FlagEnabled.ShouldBeFalse();
    }

    [Fact]
    public async Task EvaluateFeatureFlagTask_ShouldReturnEnabled_WhenGroupAndFlagAreActive()
    {
        repository.GetList(Arg.Any<Expression<Func<FeatureFlagStatuses, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(
            [
                CreateStatus(isGroupStatus: true, manualEnabled: true),
                CreateStatus(isGroupStatus: false, manualEnabled: true),
            ]);

        var result = await sut.GetFeatureFlagStatus(CreateRequest(), TestContext.Current.CancellationToken);

        result.FlagEnabled.ShouldBeTrue();
    }

    [Fact]
    public async Task EvaluateFeatureFlagTask_ShouldReturnDisabled_WhenFlagStatusIsMissing()
    {
        repository.GetList(Arg.Any<Expression<Func<FeatureFlagStatuses, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(
            [
                CreateStatus(isGroupStatus: true, manualEnabled: true),
            ]);

        var result = await sut.GetFeatureFlagStatus(CreateRequest(), TestContext.Current.CancellationToken);

        result.FlagEnabled.ShouldBeFalse();
    }

    [Fact]
    public async Task EvaluateFeatureFlagTask_ShouldUseScheduledStatus_WhenWindowIsActive()
    {
        repository.GetList(Arg.Any<Expression<Func<FeatureFlagStatuses, bool>>>(), Arg.Any<CancellationToken>())
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

        var result = await sut.GetFeatureFlagStatus(CreateRequest(), TestContext.Current.CancellationToken);

        result.FlagEnabled.ShouldBeTrue();
    }

    private static GetFeatureFlagStatus CreateRequest()
    {
        return new GetFeatureFlagStatus { GroupName = "Payments", FlagName = "NewUi", EnvironmentName = "Prod", };
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
            Group = new FeatureGroups { Name = "Payments", },
            Flag = isGroupStatus
                ? null
                : new FeatureFlags { Name = "NewUi", },
            Environment = new Environments { Name = "Prod", },
            UpdatedBy = "tester",
        };
    }
}
