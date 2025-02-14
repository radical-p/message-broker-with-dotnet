using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageBroker.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private static readonly string _filePath = "messages.txt";
        private static readonly object _lockObject = new object();
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(ILogger<MessagesController> logger)
        {
            _logger = logger;
        }

        // POST: api/messages/send
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] JsonElement messageJson)
        {
            string message = messageJson.GetString();

            if (string.IsNullOrEmpty(message))
            {
                _logger.LogError("[SendMessage] Message cannot be empty.");
                return BadRequest("Message cannot be empty.");
            }

            lock (_lockObject)
            {
                AppendMessageToFile(message);
                _logger.LogInformation($"[SendMessage] Message enqueued and saved: {message}");
            }

            return Ok(new { message = "Message enqueued and saved successfully." });
        }

        // GET: api/messages/receive
        [HttpGet("receive")]
        public async Task<IActionResult> ReceiveMessage()
        {
            lock (_lockObject)
            {
                List<string> messages = LoadMessagesFromFile();

                if (messages.Count == 0)
                {
                    _logger.LogInformation("[ReceiveMessage] No messages available.");
                    return NoContent();
                }

                var message = messages[0];
                messages.RemoveAt(0);

                SaveMessagesToFile(messages);

                _logger.LogInformation($"[ReceiveMessage] Message received: {message}");

                return Ok(new { message = message });
            }
        }

        private void AppendMessageToFile(string message)
        {
            lock (_lockObject)
            {
                System.IO.File.AppendAllText(_filePath, message + "\n");
                _logger.LogInformation($"[AppendMessageToFile] Message saved to file: {message}");
            }
        }

        // Helper method to load all messages from file
        private List<string> LoadMessagesFromFile()
        {
            var messages = new List<string>();

            if (System.IO.File.Exists(_filePath))
            {
                messages.AddRange(System.IO.File.ReadAllLines(_filePath));
                _logger.LogInformation("[LoadMessagesFromFile] Messages loaded from file.");
            }
            else
            {
                _logger.LogWarning("[LoadMessagesFromFile] No messages file found, starting fresh.");
            }

            return messages;
        }

        private void SaveMessagesToFile(List<string> messages)
        {
            System.IO.File.WriteAllLines(_filePath, messages);
            _logger.LogInformation("[SaveMessagesToFile] Updated message list saved to file.");
        }
    }
}