using System;

namespace ProducerConsumer.Interfaces
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RateLimitAttribute : Attribute
    {
        public int MaxThreads { get; }

        public RateLimitAttribute(int maxThreads)
        {
            if (maxThreads <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxThreads), "Max threads must be greater than zero.");

            MaxThreads = maxThreads;
        }
    }
}