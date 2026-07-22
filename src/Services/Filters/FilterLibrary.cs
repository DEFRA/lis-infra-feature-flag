// <copyright file="FilterLibrary.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Services.Filters;

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Lis.Infra.FeatureFlag.Database.Entities;
using Microsoft.EntityFrameworkCore;

[ExcludeFromCodeCoverage]
public static class FilterLibrary
{
    public static class Data
    {
        public static Expression<Func<FeatureFlagStatuses, bool>> ProductSpecificFilter(string? product) =>
            featureFlagStatus =>
                product != null && featureFlagStatus.Product != null &&
                EF.Functions.ILike(featureFlagStatus.Product.Name, product);

        public static Expression<Func<FeatureFlagStatuses, bool>>
            GroupWithProductSpecificFilter(string? group, string? product) =>
            featureFlagStatus =>
                group != null && EF.Functions.ILike(featureFlagStatus.Group.Name, group) && product != null &&
                featureFlagStatus.Product != null && featureFlagStatus.Product.Name == product;

        public static Expression<Func<FeatureFlagStatuses, bool>> FlagAgnosticAndSpecificFilter(string? flag) =>
            featureFlagStatus =>
                featureFlagStatus.Flag == null ||
                (flag != null && featureFlagStatus.Flag != null &&
                 EF.Functions.ILike(featureFlagStatus.Flag.Name, flag));
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
