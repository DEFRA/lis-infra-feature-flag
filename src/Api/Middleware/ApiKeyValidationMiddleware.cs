// <copyright file="ApiKeyValidationMiddleware.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Middleware;

using Lis.Infra.FeatureFlag.Api.MetaData;
using Lis.Infra.FeatureFlag.Api.Middleware.Base;
using Lis.Infra.FeatureFlag.Api.Middleware.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public partial class ApiKeyValidationMiddleware(string apiKey, ILogger<ApiKeyValidationMiddleware> logger)
    : MiddlewareBase
{
    public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        ArgumentException.ThrowIfNullOrEmpty(apiKey);

        var endpoint = context.GetEndpoint();

        if (endpoint == null)
        {
            await next(context);
            return;
        }

        var ignoreApiKeyCheck = endpoint.Metadata.GetMetadata<IgnoreApiKeyCheck>() is not null;

        if (ignoreApiKeyCheck)
        {
            await next(context);
            return;
        }

        try
        {
            var headers = context.Request.Headers;

            headers.TryGetValue(RequestHeaderNames.ApiKey, out var key);

            if (string.IsNullOrWhiteSpace(key))
            {
                await WriteJsonErrorAsync(
                    context,
                    statusCode: StatusCodes.Status400BadRequest,
                    code: "missing_header",
                    message: $"Header {RequestHeaderNames.ApiKey} is required.",
                    details: new { header = $"{RequestHeaderNames.ApiKey}" });
                return;
            }

            if (!string.Equals(key, apiKey, StringComparison.Ordinal))
            {
                await WriteJsonErrorAsync(
                    context,
                    statusCode: StatusCodes.Status400BadRequest,
                    code: "invalid_api_key",
                    message: $"Header {RequestHeaderNames.ApiKey} is not valid.",
                    details: new { header = $"{RequestHeaderNames.ApiKey}" });
                return;
            }

            await next(context);
        }
        catch (Exception ex)
        {
            LogErrorInMiddleware(logger, nameof(ApiKeyValidationMiddleware), ex);

            throw;
        }
    }
}
