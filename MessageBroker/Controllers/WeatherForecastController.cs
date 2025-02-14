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
        // File path for storing messages
        private static readonly string _filePath = "messages.txt";

        // Lock object for thread safety
        private static readonly object _lockObject = new object();

        // Logger for logging messages
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(ILogger<MessagesController> logger)
        {
            _logger = logger;
        }

        // POST: api/messages/send
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] JsonElement messageJson)
        {
            try
            {
                // Extract the message from the JSON body
                string message = messageJson.GetString();

                // Validate the message
                if (string.IsNullOrEmpty(message))
                {
                    _logger.LogError("[SendMessage] Message cannot be empty.");
                    return BadRequest("Message cannot be empty.");
                }

                // Append the message to the file in a thread-safe manner
                lock (_lockObject)
                {
                    AppendMessageToFile(message);
                    _logger.LogInformation($"[SendMessage] Message enqueued and saved: {message}");
                }

                // Return success response
                return Ok(new { message = "Message enqueued and saved successfully." });
            }
            catch (Exception ex)
            {
                // Log and handle any unexpected errors
                _logger.LogError($"[SendMessage] An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while processing the message.");
            }
        }

        // GET: api/messages/receive
        [HttpGet("receive")]
        public async Task<IActionResult> ReceiveMessage()
        {
            try
            {
                // Retrieve and process messages in a thread-safe manner
                lock (_lockObject)
                {
                    // Load all messages from the file
                    List<string> messages = LoadMessagesFromFile();

                    // Check if there are any messages
                    if (messages.Count == 0)
                    {
                        _logger.LogInformation("[ReceiveMessage] No messages available.");
                        return NoContent();
                    }

                    // Retrieve the first message (FIFO)
                    var message = messages[0];
                    messages.RemoveAt(0); // Remove the message from the list

                    // Save the updated list back to the file
                    SaveMessagesToFile(messages);

                    // Log the received message
                    _logger.LogInformation($"[ReceiveMessage] Message received: {message}");

                    // Return the message to the consumer
                    return Ok(new { message = message });
                }
            }
            catch (Exception ex)
            {
                // Log and handle any unexpected errors
                _logger.LogError($"[ReceiveMessage] An error occurred: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving the message.");
            }
        }

        // Helper method to append a message to the file
        private void AppendMessageToFile(string message)
        {
            lock (_lockObject)
            {
                // Append the message to the file
                System.IO.File.AppendAllText(_filePath, message + Environment.NewLine);

                // Log the operation
                _logger.LogInformation($"[AppendMessageToFile] Message saved to file: {message}");
            }
        }

        // Helper method to load all messages from the file
        private List<string> LoadMessagesFromFile()
        {
            var messages = new List<string>();

            // Check if the file exists
            if (System.IO.File.Exists(_filePath))
            {
                // Read all lines from the file
                messages.AddRange(System.IO.File.ReadAllLines(_filePath));

                // Log the operation
                _logger.LogInformation("[LoadMessagesFromFile] Messages loaded from file.");
            }
            else
            {
                // Log a warning if the file does not exist
                _logger.LogWarning("[LoadMessagesFromFile] No messages file found, starting fresh.");
            }

            return messages;
        }

        // Helper method to save the updated list of messages to the file
        private void SaveMessagesToFile(List<string> messages)
        {
            lock (_lockObject)
            {
                // Write all messages back to the file
                System.IO.File.WriteAllLines(_filePath, messages);

                // Log the operation
                _logger.LogInformation("[SaveMessagesToFile] Updated message list saved to file.");
            }
        }
    }
}