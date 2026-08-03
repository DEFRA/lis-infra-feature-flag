// <copyright file="RequestHeaderMappingFilter.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Filters;

using Lis.Infra.FeatureFlag.Api.Middleware.Headers;
using Lis.Infra.FeatureFlag.Models.Requests.FeatureFlags.Base;
using Microsoft.AspNetCore.Http;

public class RequestHeaderMappingFilter<T>
    : IEndpointFilter
    where T : OperationByProductAndEnvironment
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<T>().FirstOrDefault();

        if (request == null)
        {
            return Results.BadRequest("Invalid request.");
        }

        if (context.HttpContext.Request.Headers.TryGetValue(RequestHeaderNames.ProductName, out var product))
        {
            request.ProductName = product;
        }

        if (context.HttpContext.Request.Headers.TryGetValue(RequestHeaderNames.EnvironmentName, out var environment))
        {
            request.EnvironmentName = environment;
        }

        return await next(context);
    }
}
