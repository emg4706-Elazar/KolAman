using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationGate.Services;

namespace NotificationGate;

public class Program
{
    static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = 
            Host.CreateApplicationBuilder();

        builder.Services.AddHostedService<WatchWorker>();

        var host = builder.Build();

        await host.RunAsync();
    }
}