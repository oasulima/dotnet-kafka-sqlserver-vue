namespace Admin.API.Options;

public class HttpResilienceOptions
{
    public required int MaxRetryAttempts { get; set; }
    public required TimeSpan RetryDelay { get; set; }
    public required TimeSpan AttemptTimeout { get; set; }
    public required TimeSpan TotalRequestTimeout { get; set; }
    public required TimeSpan CircuitBreakerSamplingDuration { get; set; }
    public required TimeSpan HttpClientTimeout { get; set; }
}
