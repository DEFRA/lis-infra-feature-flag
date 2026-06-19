// <copyright file="ConflictException.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Exceptions;

public sealed class ConflictException(string message)
    : Exception(message);
