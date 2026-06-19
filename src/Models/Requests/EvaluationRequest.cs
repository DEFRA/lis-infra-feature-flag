// <copyright file="EvaluationRequest.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Models.Requests;

public class EvaluationRequest
{
    public required string Group { get; set; }

    public string? Flag { get; set; }

    public string? Environment { get; set; }
}
