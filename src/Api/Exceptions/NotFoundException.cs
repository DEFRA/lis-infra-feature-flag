// <copyright file="NotFoundException.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Exceptions;

public sealed class NotFoundException(string message)
    : Exception(message);
