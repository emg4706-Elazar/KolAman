using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.CompilerServices;

namespace NotificationGate.Services;

public class WatchWorker: BackgroundService
{
    private readonly string _inputFilpath =
        @"C:\Users\EHRE14\source\repos\KolAman\alert-simulator\alert-simulator\alerts";

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var watcher = CreateWatcher();

        return Task.CompletedTask;
    }


    private FileSystemWatcher CreateWatcher()
    {
        using var watcher =
            new FileSystemWatcher(_inputFilpath);

        watcher.Filter = "";
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;
        watcher.InternalBufferSize = 65536;

        watcher.Changed += OnCreated;

        Console.WriteLine("Press enter to exit.");
        Console.ReadLine();

        return watcher;
    }

    private static void OnCreated(object sender, FileSystemEventArgs eventArgs)
    {
        string value = $"Created: {eventArgs.FullPath}";
        Console.WriteLine(value);
    }
}
