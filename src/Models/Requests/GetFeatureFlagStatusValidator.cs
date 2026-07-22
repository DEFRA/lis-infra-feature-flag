// <copyright file="GetFeatureFlagStatusValidator.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Models.Requests;

using FluentValidation;

public class GetFeatureFlagStatusValidator : AbstractValidator<GetFeatureFlagStatus>
{
    public GetFeatureFlagStatusValidator()
    {
        const string environmentConstraint = @"^(?i)(DEV|TEST|EXT-TEST|PROD)$";

        RuleFor(x => x.ProductName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.EnvironmentName).NotEmpty().MaximumLength(10).Matches(environmentConstraint);
        RuleFor(x => x.GroupName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.FlagName).NotEmpty().MaximumLength(100);
    }
}
