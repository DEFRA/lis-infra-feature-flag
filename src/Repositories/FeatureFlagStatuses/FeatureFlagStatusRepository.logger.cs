// <copyright file="FeatureFlagStatusRepository.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Repositories.FeatureFlagStatuses;

using Microsoft.Extensions.Logging;

public partial class FeatureFlagStatusRepository
{
    [LoggerMessage(LogLevel.Information, "Getting list of feature flag statuses")]
    partial void LogGettingListOfFeatureFlagStatuses();
}
