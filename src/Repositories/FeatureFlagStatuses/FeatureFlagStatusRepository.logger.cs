// <copyright file="FeatureFlagStatusRepository.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Repositories.FeatureFlagStatuses;

using Microsoft.Extensions.Logging;

public partial class FeatureFlagStatusRepository
{
    [LoggerMessage(LogLevel.Information, "Getting single county parish holding")]
    partial void LogGettingSingleCountyParishHolding();

    [LoggerMessage(LogLevel.Information, "Getting list of county parish holdings")]
    partial void LogGettingListOfCountyParishHoldings();

    [LoggerMessage(LogLevel.Information, "Creating county parish holding")]
    partial void LogCreatingCountyParishHolding();

    [LoggerMessage(LogLevel.Information, "Updating county parish holding with id {Id}")]
    partial void LogUpdatingCountyParishHoldingWithId(Guid id);
}
