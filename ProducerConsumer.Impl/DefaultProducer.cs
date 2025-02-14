using ProducerConsumer.Interfaces;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProducerConsumer.Impl
{
    [RetryPolicy(retryCount: 3, retryDelaySeconds: 2)]
    [RateLimit(maxThreads: 5)] 
    public class DefaultProducer : IProducer
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly SemaphoreSlim _semaphore;

        public int RetryCount { get; private set; }
        public TimeSpan RetryDelay { get; private set; }
        public int MaxThreads { get; private set; }

        public DefaultProducer()
        {
            var retryAttr = (RetryPolicyAttribute)Attribute.GetCustomAttribute(
                typeof(DefaultProducer), typeof(RetryPolicyAttribute)
            );

            var rateLimitAttr = (RateLimitAttribute)Attribute.GetCustomAttribute(
                typeof(DefaultProducer), typeof(RateLimitAttribute)
            );

            RetryCount = retryAttr?.RetryCount ?? 0;
            RetryDelay = TimeSpan.FromSeconds(retryAttr?.RetryDelaySeconds ?? 0);
            MaxThreads = rateLimitAttr?.MaxThreads ?? 1; 

            _semaphore = new SemaphoreSlim(MaxThreads);
        }

        public async Task SendAsync(string message, string endpoint)
        {
            // Acquire a semaphore slot (wait if all threads are busy)
            await _semaphore.WaitAsync();

            try
            {
                Console.WriteLine($"[Producer] Attempting to send message to {endpoint}");

                string jsonPayload = JsonConvert.SerializeObject(message);

                for (int attempt = 0; attempt <= RetryCount; attempt++)
                {
                    try
                    {
                        var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                        var response = await _httpClient.PostAsync(endpoint, content);

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("[Producer] Message sent successfully!");
                            break; 
                        }
                        else
                        {
                            throw new Exception($"Failed to send message. Status Code: {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Producer] Attempt {attempt + 1} failed: {ex.Message}");

                        if (attempt < RetryCount)
                        {
                            Console.WriteLine($"[Producer] Waiting {RetryDelay.TotalSeconds} seconds before retry...");
                            await Task.Delay(RetryDelay);
                        }
                        else
                        {
                            Console.WriteLine("[Producer] All retry attempts failed. Aborting send.");
                        }
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}