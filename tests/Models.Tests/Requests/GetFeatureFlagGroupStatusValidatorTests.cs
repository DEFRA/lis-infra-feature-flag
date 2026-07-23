// <copyright file="GetFeatureFlagGroupStatusValidatorTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Models.Tests.Requests;

using FluentValidation.TestHelper;
using Lis.Infra.FeatureFlag.Models.Requests;

public class GetFeatureFlagGroupStatusValidatorTests
{
    private readonly GetFeatureFlagGroupStatusValidator validator = new();

    [Fact]
    public void Should_Have_Error_When_ProductName_Is_Null()
    {
        var model = CreateValidRequest();

        model.ProductName = null;

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ProductName);
    }

    [Fact]
    public void Should_Have_Error_When_ProductName_Is_Empty()
    {
        var model = CreateValidRequest();

        model.ProductName = string.Empty;

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ProductName);
    }

    [Fact]
    public void Should_Have_Error_When_ProductName_Is_WhiteSpace()
    {
        var model = CreateValidRequest();

        model.ProductName = "   ";

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ProductName);
    }

    [Fact]
    public void Should_Have_Error_When_ProductName_Exceeds_50_Characters()
    {
        var model = CreateValidRequest();

        model.ProductName = new string('a', 51);

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ProductName);
    }

    [Fact]
    public void Should_Have_Error_When_EnvironmentName_Is_Null()
    {
        var model = CreateValidRequest();

        model.EnvironmentName = null;

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.EnvironmentName);
    }

    [Fact]
    public void Should_Have_Error_When_EnvironmentName_Is_Empty()
    {
        var model = CreateValidRequest();

        model.EnvironmentName = string.Empty;

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.EnvironmentName);
    }

    [Fact]
    public void Should_Have_Error_When_EnvironmentName_Is_WhiteSpace()
    {
        var model = CreateValidRequest();

        model.EnvironmentName = "   ";

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.EnvironmentName);
    }

    [Fact]
    public void Should_Have_Error_When_EnvironmentName_Exceeds_10_Characters()
    {
        var model = CreateValidRequest();

        model.EnvironmentName = new string('a', 11);

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.EnvironmentName);
    }

    [Theory]
    [InlineData("dev")]
    [InlineData("test")]
    [InlineData("ext-test")]
    [InlineData("prod")]
    public void Should_Not_Have_Error_When_EnvironmentName_Constraint_Valid(string environmentName)
    {
        var model = CreateValidRequest();

        model.EnvironmentName = environmentName;

        var result = this.validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("das")]
    [InlineData("23424")]
    [InlineData("Production")]
    [InlineData("y8594")]
    public void Should_Have_Error_When_EnvironmentName_Constraint_Invalid(string environmentName)
    {
        var model = CreateValidRequest();

        model.EnvironmentName = environmentName;

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.EnvironmentName);
    }

    [Fact]
    public void Should_Have_Error_When_GroupName_Is_Empty()
    {
        var model = CreateValidRequest();

        model.GroupName = string.Empty;

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.GroupName);
    }

    [Fact]
    public void Should_Have_Error_When_GroupName_Is_WhiteSpace()
    {
        var model = CreateValidRequest();

        model.GroupName = "   ";

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.GroupName);
    }

    [Fact]
    public void Should_Have_Error_When_GroupName_Exceeds_100_Characters()
    {
        var model = CreateValidRequest();

        model.GroupName = new string('a', 101);

        var result = this.validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.GroupName);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Model_Is_Valid()
    {
        var model = CreateValidRequest();

        var result = this.validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static GetFeatureFlagGroupStatus CreateValidRequest() =>
        new GetFeatureFlagGroupStatus() { EnvironmentName = "prod", ProductName = "LIS", GroupName = "payments", };
}
