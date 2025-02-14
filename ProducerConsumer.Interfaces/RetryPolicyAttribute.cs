using System;

namespace ProducerConsumer.Interfaces
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RetryPolicyAttribute : Attribute
    {
        public int RetryCount { get; }
        public int RetryDelaySeconds { get; }

        public RetryPolicyAttribute(int retryCount, int retryDelaySeconds)
        {
            if (retryCount < 0)
                throw new ArgumentOutOfRangeException(nameof(retryCount), "Retry count must be non-negative.");

            if (retryDelaySeconds < 0)
                throw new ArgumentOutOfRangeException(nameof(retryDelaySeconds), "Retry delay must be non-negative.");

            RetryCount = retryCount;
            RetryDelaySeconds = retryDelaySeconds;
        }
    }
}

