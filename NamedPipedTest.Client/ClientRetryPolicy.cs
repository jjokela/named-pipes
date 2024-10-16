using Polly;
using Polly.Retry;

namespace NamedPipedTest.Client;

public class ClientRetryPolicy
{
    public static RetryPolicy GetRetryPolicy()
    {
        return Policy
            .Handle<IOException>() 
            .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry {retryCount} after {timeSpan.TotalSeconds}s due to: {exception.Message}");
                });
    }
}