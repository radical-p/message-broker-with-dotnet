using ProducerConsumer.Interfaces;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ProducerConsumer.Impl
{
    [RateLimit(maxThreads: 3)] 
    public class DefaultConsumer : IConsumer
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly SemaphoreSlim _semaphore;

        public int MaxThreads { get; private set; }

        public DefaultConsumer()
        {
            var rateLimitAttr = (RateLimitAttribute)Attribute.GetCustomAttribute(
                typeof(DefaultConsumer), typeof(RateLimitAttribute)
            );

            MaxThreads = rateLimitAttr?.MaxThreads ?? 1; 
            _semaphore = new SemaphoreSlim(MaxThreads);
        }

        public async Task ConsumeAsync(string endpoint)
        {
            await _semaphore.WaitAsync();

            try
            {
                Console.WriteLine($"[Consumer] Attempting to receive message from {endpoint}");

                while (true) 
                {
                    try
                    {
                        var response = await _httpClient.GetAsync(endpoint);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();

                            if (string.IsNullOrWhiteSpace(responseContent))
                            {
                                Console.WriteLine("[Consumer] No messages available. Retrying...");
                            }
                            else
                            {
                                var jsonResponse = JObject.Parse(responseContent);
                                string message = jsonResponse["message"]?.ToString();

                                Console.WriteLine($"[Consumer] Received message: {message}");
                                break; 
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[Consumer] API returned error: {response.StatusCode}. Retrying...");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Consumer] Error receiving message: {ex.Message}. Retrying...");
                    }

                    await Task.Delay(2000); 
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}