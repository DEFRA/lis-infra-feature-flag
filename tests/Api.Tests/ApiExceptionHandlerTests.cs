// <copyright file="ApiExceptionHandlerTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Tests;

using System.Text.Json;
using Lis.Infra.FeatureFlag.Api.Exceptions;
using Lis.Infra.FeatureFlag.Api.Middleware.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class ApiExceptionHandlerTests
{
    public static TheoryData<Exception, int, string, string> Exceptions =>
        new()
        {
            { new NotFoundException("not found"), StatusCodes.Status404NotFound, "Not Found", "https://httpstatuses.com/404" },
            { new ConflictException("conflict"), StatusCodes.Status409Conflict, "Conflict", "https://httpstatuses.com/409" },
            { new BusinessRuleException("business"), StatusCodes.Status400BadRequest, "Bad Request", "https://httpstatuses.com/400" },
            { new ArgumentException("argument"), StatusCodes.Status400BadRequest, "Bad Request", "https://httpstatuses.com/400" },
            { new UnauthorizedAccessException("forbidden"), StatusCodes.Status403Forbidden, "Forbidden", "https://httpstatuses.com/403" },
            { new InvalidOperationException("boom"), StatusCodes.Status500InternalServerError, "Internal Server Error", "https://httpstatuses.com/500" },
        };

    [Theory]
    [MemberData(nameof(Exceptions))]
    public async Task TryHandleAsync_ShouldWriteExpectedProblemDetails(
        Exception exception,
        int expectedStatusCode,
        string expectedTitle,
        string expectedType)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "trace-123";
        httpContext.Request.Method = HttpMethods.Get;
        httpContext.Request.Path = "/evaluate/payments";
        httpContext.Request.Headers[RequestHeaderNames.CorrelationId] = "corr-123";
        httpContext.Response.Body = new MemoryStream();

        var sut = new ApiExceptionHandler(Substitute.For<ILogger<ApiExceptionHandler>>());

        var handled = await sut.TryHandleAsync(httpContext, exception, TestContext.Current.CancellationToken);

        handled.ShouldBeTrue();
        httpContext.Response.StatusCode.ShouldBe(expectedStatusCode);

        httpContext.Response.Body.Position = 0;
        using var payload = await JsonDocument.ParseAsync(httpContext.Response.Body, cancellationToken: TestContext.Current.CancellationToken);

        payload.RootElement.GetProperty("status").GetInt32().ShouldBe(expectedStatusCode);
        payload.RootElement.GetProperty("title").GetString().ShouldBe(expectedTitle);
        payload.RootElement.GetProperty("type").GetString().ShouldBe(expectedType);
        payload.RootElement.GetProperty("detail").GetString().ShouldBe(exception.Message);
        payload.RootElement.GetProperty("instance").GetString().ShouldBe("/evaluate/payments");
        payload.RootElement.GetProperty("traceId").GetString().ShouldBe("trace-123");
    }
}
