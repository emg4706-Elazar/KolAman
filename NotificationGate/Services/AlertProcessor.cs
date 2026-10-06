using NotificationGate.Models;
using System.Text.Json;

namespace NotificationGate.Services;

public class AlertProcessor
{
    private readonly JsonReaderService _jsonReader;
    private readonly KafkaProducerService _producer;

    public AlertProcessor(
        JsonReaderService jsonReader,
        KafkaProducerService producer)
    {
        _jsonReader = jsonReader;
        _producer = producer;
    }


    public async Task ProcessAndSendAsync(string filepath)
    {
        Alert? alert = 
            _jsonReader.LoadJson(filepath);

        if (alert is not null)
        {
            await _producer.ProduceAsync(alert);
        }
    }
}
