// See https://aka.timeout/new-console-template for more information
using Events;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        ConnectionFactory factory = new()
        {
            Uri = new("amqp://user:bitnami@localhost:5672"),
            ClientProvidedName = "RabbitMQ Sender App"
        };

        IConnection connection = factory.CreateConnection();

        IModel channel = connection.CreateModel();

        string exchangeName = "demo-exchange";
        string routingKey = "demo-routing-key";
        string queueName = "demo-queue";

        channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

        channel.QueueDeclare(queueName,
            false,
            false,
            false,
            new Dictionary<string, object>()
            {
        {"x-max-priority", 5 }
            });

        channel.QueueBind(queueName, exchangeName, routingKey, null);
        var tasks = new List<PriorityTask>
        {
            CreatePriorityTask(0, "Task 0 with P0", 0),
            CreatePriorityTask(1, "Task 1 with P5", 5),
            CreatePriorityTask(2, "Task 2 with P1", 1),
            CreatePriorityTask(3, "Task 3 with P3", 3),
            CreatePriorityTask(4, "Task 4 with P2", 2),
            CreatePriorityTask(5, "Task 5 with P4", 4),
            CreatePriorityTask(6, "Task 6 with P5", 5),
            CreatePriorityTask(7, "Task 7 with P1", 1),
            CreatePriorityTask(8, "Task 8 with P3", 3),
            CreatePriorityTask(9, "Task 9 with P2", 2),
            CreatePriorityTask(10, "Task 10 with P4", 4)
        };


        for (int i = 0; i < tasks.Count; i++)
        {
            Console.WriteLine($"Sending Message #{i}");

            var taskSerialized = JsonSerializer.Serialize(tasks[i]);

            byte[] messageBodyBytes = Encoding.UTF8.GetBytes(taskSerialized);

            var props = channel.CreateBasicProperties();
            props.Priority = (byte)tasks[i].Priority;

            channel.BasicPublish(exchangeName, routingKey, props, messageBodyBytes);

            var timeout = TimeSpan.FromMilliseconds(500).Milliseconds;

            Thread.Sleep(timeout);
        }

        channel.Close();
        connection.Close();
    }

    private static PriorityTask CreatePriorityTask(int taskId, string desc, int priority)
    {
        return new PriorityTask(taskId,
            desc,
            priority);
    }
}