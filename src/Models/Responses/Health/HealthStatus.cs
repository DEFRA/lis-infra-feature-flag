// <copyright file="HealthStatus.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Models.Responses.Health;

public class HealthStatus
{
    public required string Status { get; set; }
}
