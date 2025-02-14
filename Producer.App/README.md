# Producer App

The **Producer App** is a console application that sends messages to the **Message Broker Web API**. It dynamically loads the producer implementation from a DLL and sends messages using a configurable number of threads.

---

## Features
1. **Dynamic Loading**:
   - The producer implementation is loaded from a DLL (`ProducerConsumer.Impl.dll`) at runtime.
   - Uses reflection to load and instantiate the producer class.

2. **Thread Management**:
   - The producer specifies the number of threads it requires using the `RateLimitAttribute`.
   - Uses `SemaphoreSlim` to enforce the thread limit.

3. **Retry Mechanism**:
   - If the Message Broker is unavailable, the producer retries sending the message with a configurable retry count and delay.

4. **Logging**:
   - Logs are written to the console using the standard `ILogger` interface.

---

## Getting Started

### Prerequisites
- An IDE (e.g., [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/))

---

### Installation
1. Clone the repository:
   ```bash
   cd message-broker
   ```

2. Build the **ProducerConsumer.Impl** project to generate the DLL:
   ```bash
   dotnet build ProducerConsumer.Impl
   ```

3. Copy the DLL to the `ProducerApp` folder:
   ```bash
   cp ProducerConsumer.Impl/bin/Debug/net6.0/ProducerConsumer.Impl.dll ProducerApp/
   ```

4. Build the **ProducerApp**:
   ```bash
   dotnet build ProducerApp
   ```

---

### Running the Application
1. Start the **Message Broker Web API** (if not already running):
   ```bash
   dotnet run --project MessageBroker.WebApi
   ```

2. Run the **ProducerApp**:
   ```bash
   dotnet run --project ProducerApp
   ```

3. The producer will start sending messages to the Message Broker using the configured number of threads.

---

### Configuration
#### Thread Management
- The producer specifies the number of threads it requires using the `RateLimitAttribute`:
  ```csharp
  [RateLimit(maxThreads: 5)]
  public class DefaultProducer : IProducer { ... }
  ```

#### Retry Mechanism
- The producer retries sending messages if the Message Broker is unavailable:
  - Retry count and delay are configured using the `RetryPolicyAttribute`:
    ```csharp
    [RetryPolicy(retryCount: 3, retryDelaySeconds: 2)]
    public class DefaultProducer : IProducer { ... }
    ```

#### Message Broker Endpoint
- The producer sends messages to the Message Broker at:
  ```
  http://localhost:5026/api/messages/send
  ```

---

### Example Output
When the producer runs, it will display logs in the console:
```
[Producer] Attempting to send message to http://localhost:5026/api/messages/send
[Producer] Message sent successfully!
[Producer] Attempt 1 failed: Failed to send message. Status Code: 500
[Producer] Waiting 2 seconds before retry...
[Producer] Message sent successfully!
```
