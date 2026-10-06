using Microsoft.Extensions.Hosting;

namespace NotificationGate.Services;

public class WorkerService: BackgroundService
{
    private readonly FileWatcher _watcher;

    public WorkerService(
        FileWatcher watcher)
    {
        _watcher = watcher;
    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _watcher.Start();

        return Task.CompletedTask;
    }
}