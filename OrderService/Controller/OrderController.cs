using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using Common.EventBus;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    //private readonly IConnection _connection;
    private readonly RabbitMQ.Client.IConnection _connection;
    public OrderController(IConnection connection)
    {
        _connection = connection;
    }

    [HttpPost]
    public IActionResult CreateOrder([FromBody] OrderCreatedEvent order)
    {
        using var channel = _connection.CreateModel();
        channel.QueueDeclare(queue: "orderQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var json = JsonConvert.SerializeObject(order);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(exchange: "", routingKey: "orderQueue", basicProperties: null, body: body);

        return Ok("Order placed successfully!");
    }
}
