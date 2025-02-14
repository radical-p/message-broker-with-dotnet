# Message Broker Project

The **Message Broker Project** is a complete system for managing message exchange between **Producers** and **Consumers**. It consists of a **Web API** acting as the Message Broker, a **Producer App**, and a **Consumer App**. The system ensures message persistence, order preservation, fault tolerance, and dynamic loading of Producers and Consumers.

---

## Features
1. **Message Broker Web API**:
   - Acts as the central hub for message exchange.
   - Stores messages in a file (`messages.txt`) for persistence.
   - Ensures messages are delivered in the order they were sent (FIFO).
   - Provides endpoints for sending and receiving messages.

2. **Producer App**:
   - Sends messages to the Message Broker.
   - Supports dynamic loading of producer implementations from a DLL.
   - Configurable retry mechanism for failed message sends.
   - Thread management for concurrent message sending.

3. **Consumer App**:
   - Receives messages from the Message Broker.
   - Supports dynamic loading of consumer implementations from a DLL.
   - Thread management for concurrent message processing.
   - Retries if the Message Broker is unavailable.

4. **Dynamic Loading**:
   - Producers and Consumers are implemented as plugins (DLLs) and loaded dynamically at runtime.

5. **Logging**:
   - Logs are written to both the **console** and a **file** (`logs/log.txt`) using **Serilog**.
   - Supports log levels: `Information`, `Warning`, and `Error`.

---

## Project Structure
```
message-broker/
├── MessageBroker/          # Web API project
│   ├── Controllers/               # API controllers
│   ├── Program.cs                 # Entry point and Serilog configuration
│   └── appsettings.json           # Configuration file
├── ProducerConsumer.Interfaces/   # Interfaces for Producers/Consumers
├── ProducerConsumer.Impl/         # Implementation of Producers/Consumers
├── ProducerApp/                   # Producer console application
├── ConsumerApp/                   # Consumer console application
└── README.md                      # This file
```

---

## Getting Started

### Prerequisites
- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- An IDE (e.g., [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/))

---

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/message-broker.git
   cd message-broker
   ```

2. Install the required NuGet packages:
   ```bash
   dotnet add package Serilog.AspNetCore
   dotnet add package Serilog.Sinks.Console
   dotnet add package Serilog.Sinks.File
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

---

### Running the Application
1. Start the **Message Broker Web API**:
   ```bash
   dotnet run --project MessageBroker.WebApi
   ```

2. Run the **ConsumerApp**:
   ```bash
   dotnet run --project ConsumerApp
   ```

3. Run the **ProducerApp**:
   ```bash
   dotnet run --project ProducerApp
   ```

---

### Testing the API
#### 1. Send a Message
Use a tool like **Postman** or **curl** to send a message:
```bash
curl -X POST http://localhost:5026/api/messages/send \
  -H "Content-Type: application/json" \
  -d '"Hello, World!"'
```

#### 2. Receive a Message
Use a tool like **Postman** or **curl** to receive a message:
```bash
curl -X GET http://localhost:5026/api/messages/receive
```

---

### Configuration
#### Logging
- Logs are written to `logs/log.txt` with daily rolling.
- Log format: `{Timestamp} [{Level}] {Message}{NewLine}{Exception}`

#### Message Storage
- Messages are stored in `messages.txt` in the project root.

#### Thread Management
- Producers and Consumers specify their thread requirements using the `RateLimitAttribute`:
  ```csharp
  [RateLimit(maxThreads: 5)]
  public class DefaultProducer : IProducer { ... }
  ```

#### Retry Mechanism
- Producers retry sending messages if the server is unavailable, with configurable retry count and delay:
  ```csharp
  [RetryPolicy(retryCount: 3, retryDelaySeconds: 2)]
  public class DefaultProducer : IProducer { ... }
  ```

---

### Logs
- **Console**: Logs are displayed in the console.
- **File**: Logs are saved in `logs/log.txt`.

Example log file:
```
2023-10-15 12:34:56.789 [Information] [SendMessage] Message enqueued and saved: Hello, World!
2023-10-15 12:35:01.123 [Information] [ReceiveMessage] Message received: Hello, World!
2023-10-15 12:35:05.456 [Warning] [LoadMessagesFromFile] No messages file found, starting fresh.
```
