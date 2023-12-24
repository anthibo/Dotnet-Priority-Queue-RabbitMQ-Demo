// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new()
{
    Uri = new("amqp://guest:guest@localhost:5672"),
    ClientProvidedName = "RabbitMQ Sender App"
};

IConnection connection = factory.CreateConnection();

IModel channel = connection.CreateModel();

string exchangeName = "demo-exchange";
string routingKey = "demo-routing-key";
string queueName = "demo-queue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey, null);

for (int i = 0; i <60; i++)
{
    Console.WriteLine($"Sending Message #{i}");
    byte[] messageBodyBytes = Encoding.UTF8.GetBytes($"Message ${i}");
    channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);

    Thread.Sleep(1000);
}

channel.Close();
connection.Close();

