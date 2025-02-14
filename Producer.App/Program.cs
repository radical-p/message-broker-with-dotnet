using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        Type producerType = assembly.GetTypes()
            .FirstOrDefault(t => typeof(IProducer).IsAssignableFrom(t) && !t.IsInterface);

        if (producerType == null)
        {
            Console.WriteLine("No valid producer found!");
            return;
        }

        IProducer producer = (IProducer)Activator.CreateInstance(producerType);

        Console.WriteLine($"\n[System] Producer configured with {producer.MaxThreads} max threads");

        List<string> messages = new List<string>
        {
            "Message 1",
            "Message 2",
            "Message 3",
            "Message 4",
            "Message 5",
            "Message 6",
            "Message 7"
        };

        string endpoint = "http://localhost:5026/api/Messages/send";

        // Send messages concurrently
        var tasks = messages.Select(async msg =>
        {
            string jsonMessage = JsonConvert.SerializeObject(msg);
            await producer.SendAsync(jsonMessage, endpoint);
        });

        await Task.WhenAll(tasks);
        Console.WriteLine("\n[System] All messages processed");
    }
}