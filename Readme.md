### Priority Queue POC with C# and RabbitMQ

This proof of concept (POC) project demonstrates the implementation of a priority queue using C# along with RabbitMQ (RMQ) messaging system. The priority queue allows tasks or messages to be processed based on their priority levels, ensuring high-priority tasks are handled first.

---

## Usage Recommendation

- Declare the queue with `x-max-priority` in both Producer and Consumer apps.
- **Priority range between 1 and 5 is highly recommended**.
- Priority range could be from 1 up to 255, but a high range is not recommended as it will require more CPU and memory resources since RMQ needs to internally maintain sub-queues for each priority up to the maximum value.

```csharp
channel.QueueDeclare(queueName,
    false,
    false,
    false,
    new Dictionary<string, object>()
    {
        {"x-max-priority", 5 } // Priorities range will be from 1 up to 5
    });
```

In the producer app:

```csharp
var props = channel.CreateBasicProperties();
props.Priority = 5; // or any int value of the priority assigned to the message;

channel.BasicPublish(exchangeName, routingKey, props, messageBodyBytes);
```

---

### Demo Results

- When adding the priority in the message props ✅:

```plaintext
Message Received {"TaskID":1,"Description":"Task 1 with P5","Priority":5}
Message Received {"TaskID":6,"Description":"Task 6 with P5","Priority":5}
Message Received {"TaskID":5,"Description":"Task 5 with P4","Priority":4}
Message Received {"TaskID":10,"Description":"Task 10 with P4","Priority":4}
Message Received {"TaskID":3,"Description":"Task 3 with P3","Priority":3}
Message Received {"TaskID":8,"Description":"Task 8 with P3","Priority":3}
Message Received {"TaskID":4,"Description":"Task 4 with P2","Priority":2}
Message Received {"TaskID":9,"Description":"Task 9 with P2","Priority":2}
Message Received {"TaskID":2,"Description":"Task 2 with P1","Priority":1}
Message Received {"TaskID":7,"Description":"Task 7 with P1","Priority":1}
```

- Higher priorities are acknowledged first.
- Execution order of priority messages:
	- 5 -> 4 -> 3 -> 2 -> 1 -> 0 (messages with no priorities)
