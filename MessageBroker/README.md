# Message Broker Web API

This is a **Message Broker Web API** built using **ASP.NET Core** and **C#**. It acts as a middleware for managing message exchange between **Producers** and **Consumers**. The system ensures message persistence, order preservation, and fault tolerance.

---

## Features
1. **Message Persistence**:
   - Messages are stored in a file (`messages.txt`) to ensure they are not lost in case of server shutdown or failure.
   - Messages are reloaded from the file when the server restarts.

2. **Order Preservation**:
   - Messages are delivered to Consumers in the same order they were sent by Producers (FIFO).

3. **Thread-Safe Operations**:
   - Uses `lock` to ensure thread safety when accessing shared resources (e.g., the message file).

4. **Logging**:
   - Logs are written to both the **console** and a **file** (`logs/log.txt`) using **Serilog**.
   - Supports log levels: `Information`, `Warning`, and `Error`.

5. **Retry Mechanism**:
   - Producers retry sending messages if the server is unavailable, with configurable retry count and delay.

---

## Getting Started

### Prerequisites
- An IDE (e.g., [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/))

---

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/radical-p/message-broker-with-dotnet.git
   cd message-broker-with-dotnet
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

2. The API will be available at:
   - **Send Message**: `POST http://localhost:5026/api/messages/send`
   - **Receive Message**: `GET http://localhost:5026/api/messages/receive`

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
