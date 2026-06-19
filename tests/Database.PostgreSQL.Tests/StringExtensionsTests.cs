// <copyright file="StringExtensionsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database.PostgreSql.Tests;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("FeatureFlagStatuses", "feature_flag_statuses")]
    [InlineData("A", "a")]
    [InlineData("ReadOnlyPostgresDbContext", "read_only_postgres_db_context")]
    public void ToSnakeCase_ShouldConvertPascalCaseToSnakeCase(string input, string expected)
    {
        input.ToSnakeCase().ShouldBe(expected);
    }
}
