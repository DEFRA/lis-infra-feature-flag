// <copyright file="FeatureFlagServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Services.Tests;

using System.Linq.Expressions;
using Defra.Livestock.Sdk.Api.Strategies.Operations;
using FluentValidation;
using FluentValidation.Results;
using Lis.Infra.FeatureFlag.Database.Domain;
using Lis.Infra.FeatureFlag.Database.Entities;
using Lis.Infra.FeatureFlag.Models.Requests.FeatureFlags;
using Lis.Infra.FeatureFlag.Repositories.FeatureFlagStatuses;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class FeatureFlagServiceTests
{
    private readonly IFeatureFlagStatusRepository repository;
    private readonly FeatureFlagService sut;

    public FeatureFlagServiceTests()
    {
        repository = Substitute.For<IFeatureFlagStatusRepository>();
        var logger = Substitute.For<ILogger<FeatureFlagService>>();
        var getFeatureFlagGroupStatusValidator = Substitute.For<IValidator<GetFeatureFlagGroupStatus>>();
        var getFeatureFlagStatusValidator = Substitute.For<IValidator<GetFeatureFlagStatus>>();

        getFeatureFlagGroupStatusValidator
            .ValidateAsync(Arg.Any<GetFeatureFlagGroupStatus>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        getFeatureFlagStatusValidator
            .ValidateAsync(Arg.Any<GetFeatureFlagStatus>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new ValidationResult()));

        sut = new FeatureFlagService(
            repository,
            new RepoStrategyFactory<FeatureFlagService>(),
            getFeatureFlagGroupStatusValidator,
            getFeatureFlagStatusValidator,
            logger);
    }

    [Fact]
    public async Task GetFeatureFlagStatus_ShouldReturnDisabled_WhenGroupIsInactive()
    {
        repository.GetList(Arg.Any<Expression<Func<FeatureFlagStatuses, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(
            [
                CreateFeatureFlagGroupStatusForProd(manualEnabled: false),
                CreateFeatureFlagAndStatusForProd(flagName: "NewUi", manualEnabled: true),
            ]);

        var result =
            await sut.GetFeatureFlagStatus(CreateGetFeatureFlagStatusRequest(), TestContext.Current.CancellationToken);

        result.FlagName.ShouldBe("NewUi");
        result.FlagEnabled.ShouldBeFalse();
        result.Success.ShouldBeTrue();
    }

    [Fact]
    public async Task GetFeatureFlagStatus_ShouldReturnEnabled_WhenGroupAndFlagAreActive()
    {
        repository.GetList(Arg.Any<Expression<Func<FeatureFlagStatuses, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(
            [
                CreateFeatureFlagGroupStatusForProd(manualEnabled: true),
                CreateFeatureFlagAndStatusForProd(flagName: "NewUi", manualEnabled: true),
            ]);

        var result =
            await sut.GetFeatureFlagStatus(CreateGetFeatureFlagStatusRequest(), TestContext.Current.CancellationToken);

        result.FlagName.ShouldBe("NewUi");
        result.FlagEnabled.ShouldBeTrue();
        result.Success.ShouldBeTrue();
    }

    [Fact]
    public async Task GetFeatureFlagStatus_ShouldReturnDisabled_WhenFlagStatusIsMissing()
    {
        repository.GetList(Arg.Any<Expression<Func<FeatureFlagStatuses, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(
            [
                CreateFeatureFlagGroupStatusForProd(manualEnabled: true),
            ]);

        var result =
            await sut.GetFeatureFlagStatus(CreateGetFeatureFlagStatusRequest(), TestContext.Current.CancellationToken);

        result.FlagName.ShouldBe("NewUi");
        result.FlagEnabled.ShouldBeFalse();
        result.Success.ShouldBeFalse();
    }

    [Fact]
    public async Task GetFeatureFlagStatus_ShouldUseScheduledStatus_WhenWindowIsActive()
    {
        repository.GetList(Arg.Any<Expression<Func<FeatureFlagStatuses, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(
            [
                CreateFeatureFlagGroupStatusForProd(
                    activationType: ActivationType.Scheduled,
                    activateAfter: DateTime.UtcNow.AddMinutes(-5),
                    expireAt: DateTime.UtcNow.AddMinutes(5)),
                CreateFeatureFlagAndStatusForProd(
                    flagName: "NewUi",
                    activationType: ActivationType.Scheduled,
                    activateAfter: DateTime.UtcNow.AddMinutes(-5),
                    expireAt: DateTime.UtcNow.AddMinutes(5)),
            ]);

        var result =
            await sut.GetFeatureFlagStatus(CreateGetFeatureFlagStatusRequest(), TestContext.Current.CancellationToken);

        result.FlagName.ShouldBe("NewUi");
        result.FlagEnabled.ShouldBeTrue();
        result.Success.ShouldBeTrue();
    }

    [Fact]
    public async Task GetFeatureFlagGroupStatus_ShouldUseStatuses_WithCorrectFlagActivation()
    {
        repository.GetList(Arg.Any<Expression<Func<FeatureFlagStatuses, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(
            [
                CreateFeatureFlagGroupStatusForProd(
                    activationType: ActivationType.Scheduled,
                    activateAfter: DateTime.UtcNow.AddMinutes(-5),
                    expireAt: DateTime.UtcNow.AddMinutes(5)),
                CreateFeatureFlagAndStatusForProd(
                    flagName: "flag-1-manual-within-window",
                    activationType: ActivationType.Manual,
                    manualEnabled: true),
                CreateFeatureFlagAndStatusForProd(
                    flagName: "flag-2-scheduled-within-window",
                    activationType: ActivationType.Scheduled,
                    activateAfter: DateTime.UtcNow.AddMinutes(-5)),
                CreateFeatureFlagAndStatusForProd(
                    flagName: "flag-3-manual-disabled",
                    activationType: ActivationType.Manual,
                    manualEnabled: false),
                CreateFeatureFlagAndStatusForProd(
                    flagName: "flag-4-scheduled-outside-window",
                    activationType: ActivationType.Scheduled,
                    activateAfter: DateTime.UtcNow.AddMinutes(5)),
                CreateFeatureFlagAndStatusForProd(
                    flagName: "flag-5-scheduled-within-window-expired",
                    activationType: ActivationType.Scheduled,
                    activateAfter: DateTime.UtcNow.AddMinutes(-10),
                    expireAt: DateTime.UtcNow.AddMinutes(-5)),
            ]);

        var result =
            await sut.GetFeatureFlagGroupStatus(
                CreateGetFeatureFlagGroupStatusRequest(),
                TestContext.Current.CancellationToken);

        result.GroupEnabled.ShouldBeTrue();
        result.GroupName.ShouldBe("Payments");
        result.Success.ShouldBeTrue();

        result.Features.ShouldNotBeNull();
        result.Features.Count.ShouldBe(5);

        result.Features[0].FlagName.ShouldBe("flag-1-manual-within-window");
        result.Features[0].FlagEnabled.ShouldBeTrue();

        result.Features[1].FlagName.ShouldBe("flag-2-scheduled-within-window");
        result.Features[1].FlagEnabled.ShouldBeTrue();

        result.Features[2].FlagName.ShouldBe("flag-3-manual-disabled");
        result.Features[2].FlagEnabled.ShouldBeFalse();

        result.Features[3].FlagName.ShouldBe("flag-4-scheduled-outside-window");
        result.Features[3].FlagEnabled.ShouldBeFalse();

        result.Features[4].FlagName.ShouldBe("flag-5-scheduled-within-window-expired");
        result.Features[4].FlagEnabled.ShouldBeFalse();
    }

    private static GetFeatureFlagStatus CreateGetFeatureFlagStatusRequest()
    {
        return new GetFeatureFlagStatus
        {
            GroupName = "Payments", FlagName = "NewUi", EnvironmentName = "Prod", ProductName = "LIS",
        };
    }

    private static GetFeatureFlagGroupStatus CreateGetFeatureFlagGroupStatusRequest()
    {
        return new GetFeatureFlagGroupStatus()
        {
            GroupName = "Payments", EnvironmentName = "Prod", ProductName = "LIS",
        };
    }

    private static FeatureFlagStatuses CreateFeatureFlagGroupStatusForProd(
        ActivationType activationType = ActivationType.Manual,
        bool? manualEnabled = true,
        DateTime? activateAfter = null,
        DateTime? expireAt = null)
    {
        var product = new Products { Name = "LIS", Id = new Guid("747a5015-cfcd-4a89-be29-4e69406511a7") };
        var environment = new Environments { Name = "Prod", Id = new Guid("6a7cd310-0f9a-47ee-8692-69b7edbd5814") };
        var group = new FeatureGroups
        {
            Name = "Payments",
            Product = product,
            ProductId = product.Id,
            Id = new Guid("eb98d0af-b33a-408a-a586-c789b86f61f4"),
        };

        return new FeatureFlagStatuses
        {
            ActivationType = activationType,
            ManualEnabled = manualEnabled,
            ActivateAfter = activateAfter,
            ExpireAt = expireAt,
            Group = group,
            GroupId = group.Id,
            Flag = null,
            EnvironmentId = environment.Id,
            Environment = environment,
            Product = product,
            ProductId = product.Id,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = "tester",
        };
    }

    private static FeatureFlagStatuses CreateFeatureFlagAndStatusForProd(
        string flagName,
        ActivationType activationType = ActivationType.Manual,
        bool? manualEnabled = true,
        DateTime? activateAfter = null,
        DateTime? expireAt = null)
    {
        var product = new Products { Name = "LIS", Id = new Guid("747a5015-cfcd-4a89-be29-4e69406511a7") };
        var environment = new Environments { Name = "Prod", Id = new Guid("6a7cd310-0f9a-47ee-8692-69b7edbd5814") };
        var group = new FeatureGroups
        {
            Name = "Payments",
            Product = product,
            ProductId = product.Id,
            Id = new Guid("eb98d0af-b33a-408a-a586-c789b86f61f4"),
        };

        return new FeatureFlagStatuses
        {
            ActivationType = activationType,
            ManualEnabled = manualEnabled,
            ActivateAfter = activateAfter,
            ExpireAt = expireAt,
            Group = group,
            GroupId = group.Id,
            Flag = new FeatureFlags
            {
                Name = flagName,
                Group = group,
                GroupId = group.Id,
                Product = product,
                ProductId = product.Id,
            },
            EnvironmentId = environment.Id,
            Environment = environment,
            Product = product,
            ProductId = product.Id,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = "tester",
        };
    }
}
