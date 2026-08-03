// <copyright file="FeatureFlagService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Services;

using Defra.Livestock.Sdk.Api.Strategies.Abstractions.Extensions;
using Defra.Livestock.Sdk.Api.Strategies.Abstractions.Operations;
using Defra.Livestock.Sdk.Api.Strategies.Operations;
using FluentValidation;
using Lis.Infra.FeatureFlag.Database.Entities;
using Lis.Infra.FeatureFlag.Database.Extensions;
using Lis.Infra.FeatureFlag.Models.Requests.FeatureFlags;
using Lis.Infra.FeatureFlag.Models.Responses.FeatureFlags;
using Lis.Infra.FeatureFlag.Models.Responses.FeatureFlags.Common;
using Lis.Infra.FeatureFlag.Repositories.FeatureFlagStatuses;
using Lis.Infra.FeatureFlag.Services.Filters;
using Lis.Infra.FeatureFlag.Services.Models;
using Microsoft.Extensions.Logging;

public class FeatureFlagService : IFeatureFlagService
{
    private readonly IFeatureFlagStatusRepository featureFlagStatusRepository;
    private readonly IRepoStrategyFactory<FeatureFlagService> strategyFactory;
    private readonly IValidator<GetFeatureFlagGroupStatus> getFeatureFlagGroupStatusValidator;
    private readonly IValidator<GetFeatureFlagStatus> getFeatureFlagStatusValidator;

    public FeatureFlagService(
        IFeatureFlagStatusRepository featureFlagStatusRepository,
        IRepoStrategyFactory<FeatureFlagService> strategyFactory,
        IValidator<GetFeatureFlagGroupStatus> getFeatureFlagGroupStatusValidator,
        IValidator<GetFeatureFlagStatus> getFeatureFlagStatusValidator,
        ILogger<FeatureFlagService> logger)
    {
        this.featureFlagStatusRepository = featureFlagStatusRepository;
        this.strategyFactory = strategyFactory;
        this.getFeatureFlagGroupStatusValidator = getFeatureFlagGroupStatusValidator;
        this.getFeatureFlagStatusValidator = getFeatureFlagStatusValidator;

        strategyFactory
            .WithDefaultEntityDescription("Feature flag statuses")
            .WithDefaultLogger(logger);
    }

    public async Task<FeatureFlagGroupStatusResult> GetFeatureFlagGroupStatus(
        GetFeatureFlagGroupStatus request,
        CancellationToken cancellationToken = default)
    {
        var featureFlagStatusFilter =
            FilterLibrary.Data.ProductSpecificFilter(request.ProductName)
                .AndAlso(FilterLibrary.Data.GroupWithProductSpecificFilter(request.GroupName, request.ProductName));

        return await strategyFactory.BuildGetListStrategy<FeatureFlagStatuses>()
            .WithActionDescription("Get feature flag group status")
            .WithRepository(featureFlagStatusRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequestValidation(() => getFeatureFlagGroupStatusValidator.ValidateAsync(request, cancellationToken))
            .WithEntityFilter(featureFlagStatusFilter)
            .ExecuteAndTransform<FeatureFlagGroupStatusResult>(statuses =>
            {
                var featureGroupAndFlagStatuses = GetFeatureGroupAndFlagStatuses(statuses, request.EnvironmentName);

                return new FeatureFlagGroupStatusResult()
                {
                    GroupName = request.GroupName,
                    GroupEnabled = featureGroupAndFlagStatuses.IsGroupEnabled,
                    Features = GetEffectiveFeatureFlagStatuses(featureGroupAndFlagStatuses),
                    Success = featureGroupAndFlagStatuses.HasGroupStatus,
                };
            });
    }

    public async Task<FeatureFlagStatusResult> GetFeatureFlagStatus(
        GetFeatureFlagStatus request,
        CancellationToken cancellationToken = default)
    {
        var featureFlagStatusFilter =
            FilterLibrary.Data.ProductSpecificFilter(request.ProductName)
                .AndAlso(FilterLibrary.Data.GroupWithProductSpecificFilter(request.GroupName, request.ProductName))
                .AndAlso(FilterLibrary.Data.FlagAgnosticAndSpecificFilter(request.FlagName));

        return await strategyFactory.BuildGetListStrategy<FeatureFlagStatuses>()
            .WithActionDescription("Get feature flag status")
            .WithRepository(featureFlagStatusRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequestValidation(() => getFeatureFlagStatusValidator.ValidateAsync(request, cancellationToken))
            .WithEntityFilter(featureFlagStatusFilter)
            .ExecuteAndTransform<FeatureFlagStatusResult>(statuses =>
            {
                var featureGroupAndFlagStatuses =
                    GetFeatureGroupAndFlagStatuses(statuses, request.EnvironmentName, request.FlagName);

                var effectiveFeatureFlagStatuses =
                    GetEffectiveFeatureFlagStatuses(featureGroupAndFlagStatuses).SingleOrDefault();

                return new FeatureFlagStatusResult()
                {
                    FlagName = request.FlagName,
                    FlagEnabled = effectiveFeatureFlagStatuses?.FlagEnabled ?? false,
                    Success = featureGroupAndFlagStatuses.GetHasFlagWithName(request.FlagName),
                };
            });
    }

    private static FeatureGroupAndFlagStatuses GetFeatureGroupAndFlagStatuses(
        List<FeatureFlagStatuses> statuses,
        string? environmentName,
        string? flagName = null)
    {
        return new FeatureGroupAndFlagStatuses()
        {
            EnvironmentAgnosticGroupStatus =
                statuses.SingleOrDefault(FilterLibrary.GroupStatusSelect.EnvironmentAgnosticGroupStatus()),
            EnvironmentSpecificGroupStatus = statuses.SingleOrDefault(
                FilterLibrary.GroupStatusSelect.EnvironmentSpecificGroupStatus(environmentName)),
            FeatureFlagStatuses = flagName != null
                ? [.. statuses.Where(FilterLibrary.FlagStatusSelect.SpecificFlag(flagName))]
                : [.. statuses.Where(FilterLibrary.FlagStatusSelect.AllFlags())],
        };
    }

    private static List<FeatureFlagStatus> GetEffectiveFeatureFlagStatuses(FeatureGroupAndFlagStatuses statuses)
    {
        if (!statuses.IsGroupEnabled)
        {
            return [];
        }

        var statusesForSpecificAndAgnosticEnvironment = statuses.GetUniqueFeatureFlagNames().Select(flagName =>
                (statuses.EnvironmentSpecificGroupStatus.IsFlagActive()
                    ? statuses.FeatureFlagStatuses.SingleOrDefault(featureFlagStatus =>
                        flagName != null &&
                        featureFlagStatus.Flag != null &&
                        featureFlagStatus.EnvironmentId != null &&
                        featureFlagStatus.Flag.Name == flagName &&
                        statuses.EnvironmentSpecificGroupStatus != null &&
                        featureFlagStatus.EnvironmentId ==
                        statuses.EnvironmentSpecificGroupStatus.EnvironmentId)
                    : null) ??
                (statuses.EnvironmentAgnosticGroupStatus.IsFlagActive()
                    ? statuses.FeatureFlagStatuses.SingleOrDefault(featureFlagStatus =>
                        flagName != null &&
                        featureFlagStatus.Flag != null &&
                        featureFlagStatus.Flag.Name == flagName &&
                        featureFlagStatus.Environment == null)
                    : null))
            .Where(featureFlagStatus => featureFlagStatus != null)
            .Select(featureFlagStatus => new FeatureFlagStatus()
            {
                FlagName = featureFlagStatus!.Flag!.Name, FlagEnabled = featureFlagStatus.IsFlagActive(),
            })
            .ToList();

        var statusesForOtherEnvironments =
            statuses.GetUniqueFeatureFlagNames().Where(flagName =>
                    flagName != null &&
                    statusesForSpecificAndAgnosticEnvironment.All(status => status.FlagName != flagName) &&
                    statuses.GetHasFlagWithName(flagName))
                .Select(flagName => new FeatureFlagStatus() { FlagName = flagName!, FlagEnabled = false });

        return statusesForSpecificAndAgnosticEnvironment.Concat(statusesForOtherEnvironments)
            .OrderBy(status => status.FlagName).ToList();
    }
}
