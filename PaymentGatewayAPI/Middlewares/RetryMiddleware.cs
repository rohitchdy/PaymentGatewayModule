using Polly.Retry;
using Polly;
using Serilog;

namespace PaymentGatewayAPI.Middlewares;

public class RetryMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AsyncRetryPolicy _retryPolicy;

    public RetryMiddleware(RequestDelegate next)
    {
        _next = next;

        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(2),
                onRetry: (exception, retryCount, context) =>
                {
                    Log.Error($"Retry {retryCount} due to: {exception.Message}");
                });
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _next(context);
            });
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync($"An error occurred: {ex.Message}");
        }
    }
}
