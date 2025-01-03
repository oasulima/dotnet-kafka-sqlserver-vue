namespace Admin.API.Options;

public class HttpResilienceOptions
{
  public int MaxRetryAttempts { get; set; }
  public TimeSpan RetryDelay { get; set; }
  public TimeSpan AttemptTimeout { get; set; }
  public TimeSpan TotalRequestTimeout { get; set; }
  public TimeSpan CircuitBreakerSamplingDuration { get; set; }
  public TimeSpan HttpClientTimeout { get; set; }
}
