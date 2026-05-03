using System.Text.Json;
using boarding_school_api.Middleware;
using boarding_school_api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace boarding_school_api.Tests;

public class ErrorHandlingMiddlewareTests
{
    private readonly Mock<ILoggingService> _loggerMock = new();

    [Fact]
    public async Task KeyNotFoundException_Returns404_LogsWarn()
    {
        var (statusCode, level) = await InvokeWithException(new KeyNotFoundException("לא נמצא"));

        Assert.Equal(404, statusCode);
        AssertLogged("WARN");
    }

    [Fact]
    public async Task InvalidOperationException_Returns409_LogsWarn()
    {
        var (statusCode, _) = await InvokeWithException(new InvalidOperationException("כבר קיים"));

        Assert.Equal(409, statusCode);
        AssertLogged("WARN");
    }

    [Fact]
    public async Task ArgumentException_Returns400_LogsWarn()
    {
        var (statusCode, _) = await InvokeWithException(new ArgumentException("קלט שגוי"));

        Assert.Equal(400, statusCode);
        AssertLogged("WARN");
    }

    [Fact]
    public async Task UnknownException_Returns500_LogsError()
    {
        var (statusCode, _) = await InvokeWithException(new Exception("crash"));

        Assert.Equal(500, statusCode);
        AssertLogged("ERROR");
    }

    [Fact]
    public async Task NoException_PassesThrough_DoesNotLog()
    {
        var context = BuildContext();
        var middleware = BuildMiddleware(_ => Task.CompletedTask);

        await middleware.Invoke(context);

        Assert.Equal(200, context.Response.StatusCode);
        _loggerMock.Verify(l => l.LogAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    // ── helpers ───────────────────────────────────────────────────────────

    private async Task<(int StatusCode, string Body)> InvokeWithException(Exception ex)
    {
        var context = BuildContext();
        var middleware = BuildMiddleware(_ => throw ex);

        await middleware.Invoke(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        return (context.Response.StatusCode, body);
    }

    private ErrorHandlingMiddleware BuildMiddleware(RequestDelegate next) =>
        new(next, NullLogger<ErrorHandlingMiddleware>.Instance, _loggerMock.Object);

    private static DefaultHttpContext BuildContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private void AssertLogged(string expectedLevel) =>
        _loggerMock.Verify(
            l => l.LogAsync(expectedLevel, It.IsAny<string>()),
            Times.Once);
}
