using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace BancoApi.Services;

public class KafkaProducerService : IDisposable
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducerService(IConfiguration configuration)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092"
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task ProduceAsync(string topic, object message)
    {
        var json = JsonSerializer.Serialize(message);

        await _producer.ProduceAsync(topic, new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = json
        });
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}
