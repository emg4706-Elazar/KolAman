using Confluent.Kafka;
using NotificationGate.Models;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using NotificationGate.Configuration;
using Microsoft.Extensions.Options;

namespace NotificationGate.Services;

public class KafkaProducerService : IDisposable
{
    private readonly KafkaOptions _options;
    private readonly IProducer<Null, string> _producer;
    private readonly ILogger<KafkaProducerService> _logger;


    public KafkaProducerService(
        IOptions<KafkaOptions> options,
        ILogger<KafkaProducerService> logger)
    {
        _options = options.Value;
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            ClientId = _options.ClientId
        };

        _producer = new ProducerBuilder<
            Null, string>(config)
            .Build();
    }


    public async Task ProduceAsync(Alert alert)
    {
        string jsonMessage;

        try
        {
            jsonMessage = JsonSerializer
                .Serialize(alert);
        }
        catch (JsonException e)
        {
            _logger.LogWarning(
                e.Message,
                "Producer faild to serialize to JSON , id: {Id}",
                alert.Alertid);

            return;
        }

        if (jsonMessage is null)
            return;


        Message<Null, string> message = new()
        {
            Value = jsonMessage
        };

        await _producer.ProduceAsync(
            _options.TopicName,
            message);

        _logger.LogInformation(
            "Send: message '{Id}' to topic {Topic}",
            alert.Alertid,
            _options.TopicName);
    }

    public void Dispose()
    {
        _producer.Flush();
        _producer.Dispose();
    }
}
