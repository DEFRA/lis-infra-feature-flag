// <copyright file="ActivationType.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Database.Domain;

public enum ActivationType
{
    /// <summary>
    /// Feature flag is activated and deactivated manually
    /// </summary>
    Manual,

    /// <summary>
    /// // Feature flag is activated and deactivated by schedule
    /// </summary>
    Scheduled,
}
