using Polly;
using Polly.Retry;

namespace NamedPipedTest.Server;

public class ServerRetryPolicy
{
    public static AsyncRetryPolicy GetRetryPolicyAsync()
    {
        return Policy
            .Handle<IOException>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry {retryCount} after {timeSpan.TotalSeconds}s due to: {exception.Message}");
                });
    }
}