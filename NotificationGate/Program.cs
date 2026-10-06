using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationGate.Services;
using NotificationGate.Configuration;
using Confluent.Kafka;

namespace NotificationGate;

public class Program
{
    static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = 
            Host.CreateApplicationBuilder();


        builder.Services.AddOptions<KafkaOptions>()
            .Bind(builder.Configuration.GetSection("Kafka"))
            .Validate(options =>
                !string.IsNullOrWhiteSpace(options.BootstrapServers),
                "Kafka:BootstrapServers is required")
            .Validate(options =>
                !string.IsNullOrWhiteSpace(options.ClientId),
                "Kafka:ClientId is required")
            .Validate(options =>
                !string.IsNullOrWhiteSpace(options.TopicName),
                "Kafka:TopicName is required")
            .ValidateOnStart();


        builder.Services.AddSingleton<KafkaProducerService>();

        builder.Services.AddSingleton<FileWatcher>();

        builder.Services.AddHostedService<WorkerService>();

        builder.Services.AddSingleton<JsonReaderService>();

        builder.Services.AddSingleton<AlertProcessor>();

        
        var host = builder.Build();

        await host.RunAsync();
    }
}