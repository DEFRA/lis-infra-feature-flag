// <copyright file="FilterLibrary.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Services.Filters;

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Lis.Infra.FeatureFlag.Database.Entities;

[ExcludeFromCodeCoverage]
public static class FilterLibrary
{
    public static class Data
    {
        public static Expression<Func<FeatureFlagStatuses, bool>> ProductSpecificFilter(string? productName) =>
            featureFlagStatus =>
                productName != null && featureFlagStatus.Product != null &&
                featureFlagStatus.Product.Name == productName;

        public static Expression<Func<FeatureFlagStatuses, bool>>
            GroupWithProductSpecificFilter(string? groupName, string? productName) =>
            featureFlagStatus =>
                groupName != null && featureFlagStatus.Group.Name == groupName && productName != null &&
                featureFlagStatus.Product != null && featureFlagStatus.Product.Name == productName &&
                featureFlagStatus.Group.Product != null &&
                featureFlagStatus.Group.Product.Id == featureFlagStatus.Product.Id;

        public static Expression<Func<FeatureFlagStatuses, bool>> FlagAgnosticAndSpecificFilter(string? flagName) =>
            featureFlagStatus =>
                featureFlagStatus.Flag == null ||
                (flagName != null && featureFlagStatus.Flag != null && featureFlagStatus.Flag.Name == flagName);
    }

    public static class GroupStatusSelect
    {
        public static Func<FeatureFlagStatuses, bool> EnvironmentAgnosticGroupStatus() =>
            featureFlagStatus =>
                featureFlagStatus.Environment == null && featureFlagStatus.Flag == null;

        public static Func<FeatureFlagStatuses, bool> EnvironmentSpecificGroupStatus(string? environmentName) =>
            featureFlagStatus =>
                featureFlagStatus.Environment != null &&
                featureFlagStatus.Environment.Name == environmentName && featureFlagStatus.Flag == null;
    }

    public static class FlagStatusSelect
    {
        public static Func<FeatureFlagStatuses, bool> AllFlags() =>
            featureFlagStatus => featureFlagStatus.Flag != null;

        public static Func<FeatureFlagStatuses, bool> SpecificFlag(string? flagName) =>
            featureFlagStatus => flagName != null && featureFlagStatus.Flag != null &&
                                 featureFlagStatus.Flag.Name == flagName;
    }
}
