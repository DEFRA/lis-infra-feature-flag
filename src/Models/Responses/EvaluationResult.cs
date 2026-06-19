// <copyright file="EvaluationResult.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Models.Responses;

public record EvaluationResult()
{
    public bool IsEnabled { get; set; }

    public bool Success { get; set; }
}
