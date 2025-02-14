# Consumer App

The **Consumer App** is a console application that consumes messages from the **Message Broker Web API**. It dynamically loads the consumer implementation from a DLL and processes messages using a configurable number of threads.

---

## Features
1. **Dynamic Loading**:
   - The consumer implementation is loaded from a DLL (`ProducerConsumer.Impl.dll`) at runtime.
   - Uses reflection to load and instantiate the consumer class.

2. **Thread Management**:
   - The consumer specifies the number of threads it requires using the `RateLimitAttribute`.
   - Uses `SemaphoreSlim` to enforce the thread limit.

3. **Logging**:
   - Logs are written to the console using the standard `ILogger` interface.

4. **Retry Mechanism**:
   - If the Message Broker is unavailable, the consumer waits and retries until the server is back online.

---

## Getting Started

### Prerequisites
- An IDE (e.g., [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/))

---

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/message-broker-webapi.git
   cd message-broker-webapi
   ```

2. Build the **ProducerConsumer.Impl** project to generate the DLL:
   ```bash
   dotnet build ProducerConsumer.Impl
   ```

3. Copy the DLL to the `ConsumerApp` folder:
   ```bash
   cp ProducerConsumer.Impl/bin/Debug/net6.0/ProducerConsumer.Impl.dll ConsumerApp/
   ```

4. Build the **ConsumerApp**:
   ```bash
   dotnet build ConsumerApp
   ```

---

### Running the Application
1. Start the **Message Broker Web API** (if not already running):
   ```bash
   dotnet run --project MessageBroker.WebApi
   ```

2. Run the **ConsumerApp**:
   ```bash
   dotnet run --project ConsumerApp
   ```

3. The consumer will start polling the Message Broker for messages and process them using the configured number of threads.

---

### Configuration
#### Thread Management
- The consumer specifies the number of threads it requires using the `RateLimitAttribute`:
  ```csharp
  [RateLimit(maxThreads: 3)]
  public class DefaultConsumer : IConsumer { ... }
  ```

#### Message Broker Endpoint
- The consumer connects to the Message Broker at:
  ```
  http://localhost:5026/api/messages/receive
  ```

---

### Example Output
When the consumer runs, it will display logs in the console:
```
[Consumer] Attempting to receive message from http://localhost:5026/api/messages/receive
[Consumer] Received message: Hello, World!
[Consumer] No messages available. Retrying...
```

---
