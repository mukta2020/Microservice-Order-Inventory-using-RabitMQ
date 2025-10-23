using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<RabbitMQ.Client.IConnection>(sp =>
{
    var factory = new ConnectionFactory() { HostName = "localhost" };
    return factory.CreateConnection();
});

builder.Services.AddControllers();
var app = builder.Build();

app.MapControllers();
app.Run();
