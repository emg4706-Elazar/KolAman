using ControlSystem.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ControlSystem.Workers;

public class NorthWorker: BackgroundService
{
    private readonly RabbitOptions _options;

    public NorthWorker(
        RabbitOptions options)
    {
        _options = options;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();


        await channel.QueueDeclareAsync(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete:
            false, arguments: new Dictionary<string, object?> {
                { "x-queue-type", "quorum" }
            });

        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false
            );

        Console.WriteLine(" [*] Waiting for messages.");


        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received {message}");

            int dots = message.Split('.').Length - 1;
            await Task.Delay(dots * 1000);

            Console.WriteLine(" [x] Done");

            // here channel could also be accessed as ((AsyncEventingBasicConsumer)sender).Channel
            await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        await channel.BasicConsumeAsync("task_queue", autoAck: false, consumer: consumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}
