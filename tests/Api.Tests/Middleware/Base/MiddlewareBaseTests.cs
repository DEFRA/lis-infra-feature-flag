// <copyright file="MiddlewareBaseTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Lis.Infra.FeatureFlag.Api.Tests.Middleware.Base;

using System.Text.Json;
using Lis.Infra.FeatureFlag.Api.Middleware.Base;
using Microsoft.AspNetCore.Http;

public class MiddlewareBaseTests
{
    [Fact]
    public async Task WriteJsonErrorAsync_Writes_Correct_Response()
    {
        // Arrange
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() },
            TraceIdentifier = "test-trace-id",
            Request = { Path = "/test-path" },
        };

        var middleware = new TestJsonErrorMiddleware();

        // Act
        await middleware.InvokeAsync(context, _ => Task.CompletedTask);

        // Assert
        context.Response.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        context.Response.ContentType.ShouldBe("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);

        var json = JsonDocument.Parse(responseBody);
        var error = json.RootElement.GetProperty("error");

        error.ShouldSatisfyAllConditions(
            x => x.GetProperty("code").GetString().ShouldNotBeNullOrWhiteSpace(),
            x => x.GetProperty("message").GetString().ShouldNotBeNullOrWhiteSpace(),
            x => x.GetProperty("traceId").GetString().ShouldNotBeNullOrWhiteSpace(),
            x => x.GetProperty("path").GetString().ShouldNotBeNullOrWhiteSpace(),
            x => x.GetProperty("details").GetProperty("Detail").GetString().ShouldNotBeNullOrWhiteSpace(),
            x => x.GetProperty("code").GetString().ShouldBe("TestCode"),
            x => x.GetProperty("message").GetString().ShouldBe("TestMessage"),
            x => x.GetProperty("traceId").GetString().ShouldBe("test-trace-id"),
            x => x.GetProperty("path").GetString().ShouldBe("/test-path"),
            x => x.GetProperty("details").GetProperty("Detail").GetString().ShouldBe("TestDetail"));
    }

    private class TestJsonErrorMiddleware : MiddlewareBase
    {
        public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await WriteJsonErrorAsync(
                context,
                StatusCodes.Status400BadRequest,
                "TestCode",
                "TestMessage",
                new { Detail = "TestDetail" });
        }
    }
}
