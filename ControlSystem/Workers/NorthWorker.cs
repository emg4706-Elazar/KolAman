using ControlSystem.Configuration;
using ControlSystem.Models;
using ControlSystem.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ControlSystem.Workers;


public class NorthWorker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "app",
            Password = "secret"
        };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();


        await channel.QueueDeclareAsync(
            queue: "north",
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

            Console.WriteLine($" [x] Received message {message} from 'north'");


            int dots = message.Split('.').Length - 1;
            await Task.Delay(dots * 1000);

            Console.WriteLine(" [x] Done");

            await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        await channel.BasicConsumeAsync("north", autoAck: false, consumer: consumer);
    }
}
