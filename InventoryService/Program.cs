using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "inventoryQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var json = Encoding.UTF8.GetString(body);
    var inventoryEvent = JsonConvert.DeserializeObject<dynamic>(json);

    Console.WriteLine($"Updating inventory for OrderId: {inventoryEvent.OrderId}");
};

channel.BasicConsume(queue: "inventoryQueue", autoAck: true, consumer: consumer);

Console.WriteLine("Inventory Service running...");
Console.ReadLine();
