

namespace NotificationGate.Configuration;

public class KafkaOptions
{
    public string BootstrapServers { get; set; } = null!;
    public string TopicName { get; set; } = null!;
    public string ClientId { get; set; } = null!;
}
