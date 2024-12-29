using Refit;
using Admin.API.Options;

namespace Admin.API;

public static class RefitConfiguration
{
    public static void RegisterExternalAPI<TExternalAPI>(this IServiceCollection services, string externalServiceUrl, HttpResilienceOptions httpResilienceOptions) where TExternalAPI : class
    {
        services
            .AddRefitClient<TExternalAPI>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(externalServiceUrl);
                c.Timeout = httpResilienceOptions.HttpClientTimeout;
            })
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = httpResilienceOptions.MaxRetryAttempts;
                options.Retry.Delay = httpResilienceOptions.RetryDelay;
                options.AttemptTimeout.Timeout = httpResilienceOptions.AttemptTimeout;
                options.TotalRequestTimeout.Timeout = httpResilienceOptions.TotalRequestTimeout;
                options.CircuitBreaker.SamplingDuration = httpResilienceOptions.CircuitBreakerSamplingDuration;
            });
    }
}
