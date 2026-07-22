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
using Lis.Infra.FeatureFlag.Models.Requests;
using Lis.Infra.FeatureFlag.Models.Responses;
using Lis.Infra.FeatureFlag.Repositories.FeatureFlagStatuses;
using Lis.Infra.FeatureFlag.Services.Filters;
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
                .AndAlso(FilterLibrary.Data.EnvironmentAgnosticAndSpecificFilter(request.EnvironmentName))
                .AndAlso(FilterLibrary.Data.GroupWithProductSpecificFilter(request.GroupName, request.ProductName));

        return await strategyFactory.BuildGetListStrategy<FeatureFlagStatuses>()
            .WithActionDescription("Get feature flag group status")
            .WithRepository(featureFlagStatusRepository)
            .WithCancellationToken(cancellationToken)
            .WithRequestValidation(() => getFeatureFlagGroupStatusValidator.ValidateAsync(request, cancellationToken))
            .WithEntityFilter(featureFlagStatusFilter)
            .ExecuteAndTransform<FeatureFlagGroupStatusResult>(statuses =>
            {
                var environmentAgnosticGroupStatus =
                    statuses.SingleOrDefault(FilterLibrary.GroupStatusSelect.EnvironmentAgnosticGroupStatus());

                var environmentSpecificGroupStatus =
                    statuses.SingleOrDefault(
                        FilterLibrary.GroupStatusSelect.EnvironmentSpecificGroupStatus(request.EnvironmentName));

                var allFlags = statuses.Where(FilterLibrary.FlagStatusSelect.AllFlags()).ToList();
                var allFlagNames = allFlags.Select(status => status.Flag?.Name)
                    .Distinct();

                return new FeatureFlagGroupStatusResult()
                {
                    GroupName = request.GroupName,
                    GroupEnabled = (environmentSpecificGroupStatus ?? environmentAgnosticGroupStatus).IsFlagActive(),
                    Features = GetEffectiveFeatureFlagStatuses(
                        environmentAgnosticGroupStatus,
                        environmentSpecificGroupStatus,
                        allFlags.ToList(),
                        allFlagNames.ToArray()),
                };
            });
    }

    public async Task<FeatureFlagStatusResult> GetFeatureFlagStatus(
        GetFeatureFlagStatus request,
        CancellationToken cancellationToken = default)
    {
        var featureFlagStatusFilter =
            FilterLibrary.Data.ProductSpecificFilter(request.ProductName)
                .AndAlso(FilterLibrary.Data.EnvironmentAgnosticAndSpecificFilter(request.EnvironmentName))
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
                var environmentAgnosticGroupStatus =
                    statuses.SingleOrDefault(FilterLibrary.GroupStatusSelect.EnvironmentAgnosticGroupStatus());

                var environmentSpecificGroupStatus =
                    statuses.SingleOrDefault(
                        FilterLibrary.GroupStatusSelect.EnvironmentSpecificGroupStatus(request.EnvironmentName));

                var specificFLags = statuses.Where(FilterLibrary.FlagStatusSelect.SpecificFlag(request.FlagName))
                    .ToList();
                var specificFlagNames = specificFLags.Select(status => status.Flag?.Name)
                    .Distinct();

                return GetEffectiveFeatureFlagStatuses(
                        environmentAgnosticGroupStatus,
                        environmentSpecificGroupStatus,
                        specificFLags.ToList(),
                        specificFlagNames.ToArray())
                    .SingleOrDefault() ?? new FeatureFlagStatusResult()
                {
                    FlagName = request.FlagName, FlagEnabled = false,
                };
            });
    }

    private static List<FeatureFlagStatusResult> GetEffectiveFeatureFlagStatuses(
        FeatureFlagStatuses? environmentAgnosticGroupStatus,
        FeatureFlagStatuses? environmentSpecificGroupStatus,
        List<FeatureFlagStatuses> featureFlagStatuses,
        params string?[] flagNames)
    {
        return (environmentSpecificGroupStatus ?? environmentAgnosticGroupStatus).IsFlagActive()
            ? flagNames.Select(flagName =>
                    (environmentSpecificGroupStatus.IsFlagActive()
                        ? featureFlagStatuses.SingleOrDefault(featureFlagStatus =>
                            flagName != null &&
                            featureFlagStatus.Flag != null &&
                            featureFlagStatus.EnvironmentId != null &&
                            featureFlagStatus.Flag.Name == flagName &&
                            environmentSpecificGroupStatus != null &&
                            featureFlagStatus.EnvironmentId ==
                            environmentSpecificGroupStatus.EnvironmentId)
                        : null) ??
                    (environmentAgnosticGroupStatus.IsFlagActive()
                        ? featureFlagStatuses.SingleOrDefault(featureFlagStatus =>
                            flagName != null &&
                            featureFlagStatus.Flag != null &&
                            featureFlagStatus.Flag.Name == flagName &&
                            featureFlagStatus.Environment == null)
                        : null))
                .Where(featureFlagStatus => featureFlagStatus != null)
                .Select(featureFlagStatus => new FeatureFlagStatusResult()
                {
                    FlagName = featureFlagStatus!.Flag!.Name, FlagEnabled = featureFlagStatus.IsFlagActive(),
                })
                .ToList()
            : [];
    }
}
