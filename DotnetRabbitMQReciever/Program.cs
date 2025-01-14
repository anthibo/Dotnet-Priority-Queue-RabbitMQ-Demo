﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;



ConnectionFactory factory = new()
{
    Uri = new("amqp://user:bitnami@localhost:5672"),
    ClientProvidedName = "RabbitMQ  "
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
channel.BasicQos(0, 1, false);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (sender, args) =>
{
    Task.Delay(TimeSpan.FromSeconds(2)).Wait();

    var body = args.Body.ToArray();
    string message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"Message Received {message}");

    channel.BasicAck(args.DeliveryTag, false);
};

string consumerTag = channel.BasicConsume(queueName, false, consumer);

Console.ReadLine();

channel.BasicCancel(consumerTag);

channel.Close();
connection.Close();
