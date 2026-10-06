using ControlSystem.Configuration;
using ControlSystem.Repositories;
using ControlSystem.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;


namespace ControlSystem;

public class Program
{
    static async Task Main(string[] args)
    {
        var builder = 
            Host.CreateApplicationBuilder();

        builder.Services.AddOptions<RabbitOptions>()
            .Bind(builder.Configuration.GetSection("Rabbit"));

        builder.Services.AddOptions<MongoOptions>()
            .Bind(builder.Configuration.GetSection("Mongo"))
            .Validate(options =>
                !string.IsNullOrWhiteSpace(options.ConnectionString),
                "Mongo:ConnectionString is required")
            .Validate(options =>
                !string.IsNullOrWhiteSpace(options.Database),
                "Nongo:Database is required")
            .ValidateOnStart();


        builder.Services.AddSingleton<IMongoRepository, MongoRepository>();

        builder.Services.AddSingleton<IMongoDatabase>(factory =>
        {
            MongoOptions options =
            factory.GetRequiredService<
                MongoOptions>();

            var client = new MongoClient(options.ConnectionString);

            return client.GetDatabase(options.Database);
        });


        builder.Services.AddHostedService<NorthWorker>();


        var host = builder.Build();

        await host.RunAsync();
    }
}