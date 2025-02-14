using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ProducerConsumer.Interfaces;

class Program
{
    static async Task Main()
    {
        string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProducerConsumer.Impl.dll");

        if (!File.Exists(dllPath))
        {
            Console.WriteLine("Error: ProducerConsumer.Impl.dll not found!");
            return;
        }

        Assembly assembly = Assembly.LoadFrom(dllPath);
        Type consumerType = assembly.GetTypes()
            .FirstOrDefault(t => typeof(IConsumer).IsAssignableFrom(t) && !t.IsInterface);

        if (consumerType == null)
        {
            Console.WriteLine("No valid consumer found!");
            return;
        }

        IConsumer consumer = (IConsumer)Activator.CreateInstance(consumerType);

        Console.WriteLine($"\n[System] Consumer configured with {consumer.MaxThreads} max threads");

        var tasks = new List<Task>();
        string endpoint = "http://localhost:5026/api/Messages/receive";

        for (int i = 0; i < 10; i++) 
        {
            tasks.Add(Task.Run(async () =>
            {
                await consumer.ConsumeAsync(endpoint);
            }));
        }

        await Task.WhenAll(tasks);
        Console.WriteLine("\n[System] All messages processed");
    }
}