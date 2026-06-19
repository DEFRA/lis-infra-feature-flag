// <copyright file="BusinessRuleException.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Exceptions;

public sealed class BusinessRuleException(string message)
    : Exception(message);
