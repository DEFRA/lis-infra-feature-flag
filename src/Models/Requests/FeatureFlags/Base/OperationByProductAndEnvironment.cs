// <copyright file="OperationByProductAndEnvironment.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Models.Requests.FeatureFlags.Base;

public abstract class OperationByProductAndEnvironment
{
    public string? ProductName { get; set; }

    public string? EnvironmentName { get; set; }
}
