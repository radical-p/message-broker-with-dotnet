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

        // Load the producer implementation from the DLL
        Assembly assembly = Assembly.LoadFrom(dllPath);
        Type producerType = assembly.GetTypes()
            .FirstOrDefault(t => typeof(IProducer).IsAssignableFrom(t) && !t.IsInterface);

        if (producerType == null)
        {
            Console.WriteLine("No valid producer found!");
            return;
        }

        // Create an instance of the producer
        IProducer producer = (IProducer)Activator.CreateInstance(producerType);

        Console.WriteLine($"\n[System] Producer configured with {producer.MaxThreads} max threads");

        // List of messages to send
        List<string> messages = new List<string>
        {
            "Message 1",
            "Message 2",
            "Message 3",
            "Message 4"
        };

        string endpoint = "http://localhost:5026/api/Messages/send";

        // Send messages sequentially
        foreach (var msg in messages)
        {
            string jsonMessage = JsonConvert.SerializeObject(msg);
            await producer.SendAsync(jsonMessage, endpoint);
        }

        Console.WriteLine("\n[System] All messages processed");
    }
}