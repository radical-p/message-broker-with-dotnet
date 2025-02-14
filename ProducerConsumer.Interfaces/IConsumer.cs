using System.Threading.Tasks;

namespace ProducerConsumer.Interfaces
{
    public interface IConsumer
    {
        /// <summary>
        /// Consumes a message from the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The target endpoint (e.g., API URL).</param>
        Task ConsumeAsync(string endpoint);

        int MaxThreads { get; } // New property
    }
}