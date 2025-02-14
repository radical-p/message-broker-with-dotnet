using System;
using System.Threading.Tasks;

namespace ProducerConsumer.Interfaces
{
    public interface IProducer
    {
        /// <summary>
        /// Sends a message to the specified endpoint.
        /// </summary>
        /// <param name="message">The message content.</param>
        /// <param name="endpoint">The target endpoint (e.g., API URL).</param>
        Task SendAsync(string message, string endpoint);

        int RetryCount { get; }
        TimeSpan RetryDelay { get; }
        int MaxThreads { get; } 
    }
}
