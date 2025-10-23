using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using Common.EventBus;

var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "orderQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var json = Encoding.UTF8.GetString(body);
    var order = JsonConvert.DeserializeObject<OrderCreatedEvent>(json);

    Console.WriteLine($"Processing payment for OrderId: {order.OrderId}");

    // Publish event to Inventory
    var inventoryEvent = new { order.OrderId, order.Quantity };
    var message = JsonConvert.SerializeObject(inventoryEvent);
    var inventoryBody = Encoding.UTF8.GetBytes(message);

    channel.QueueDeclare(queue: "inventoryQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    channel.BasicPublish(exchange: "", routingKey: "inventoryQueue", basicProperties: null, body: inventoryBody);
};

channel.BasicConsume(queue: "orderQueue", autoAck: true, consumer: consumer);

Console.WriteLine("Payment Service running...");
Console.ReadLine();
