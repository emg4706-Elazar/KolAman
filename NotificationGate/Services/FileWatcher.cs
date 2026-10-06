using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;



namespace NotificationGate.Services;

public class FileWatcher: BackgroundService
{
    private readonly string _inputFilpath =
        @"C:\Users\EHRE14\source\repos\KolAman\alert-simulator\alert-simulator\alerts";
    private readonly AlertProcessor _alertProcessor;
    private readonly FileSystemWatcher _watcher;
    private readonly ILogger<FileWatcher> _logger;


    public FileWatcher(
        AlertProcessor alertProcessor,
        ILogger<FileWatcher> logger)
    {
        _alertProcessor = alertProcessor;
        _logger = logger;

        _watcher = new FileSystemWatcher(_inputFilpath)
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime,

            Filter = "*.ready",

            IncludeSubdirectories = true,
            InternalBufferSize = 65536
        };
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {

        _watcher.Created += OnReadyFileCreated;

        _watcher.EnableRaisingEvents = true;
        _watcher.IncludeSubdirectories = true;

        return Task.CompletedTask;
    }



    private async void OnReadyFileCreated(object sender, FileSystemEventArgs e)
    {
        try
        {
            var baseFileName =
                Path.GetFileNameWithoutExtension(e.FullPath);

            var directory = Path.GetDirectoryName(e.FullPath);

            var jsonFilePath = Path.Combine(directory!, $"{baseFileName}.json");

            string? targetFilePath = null;

            if (File.Exists(jsonFilePath))
            {
                targetFilePath = jsonFilePath;
                Console.WriteLine($"Found: {targetFilePath}");
            }


            if (targetFilePath is not null)
            {
                await _alertProcessor.ProcessAndSendAsync(targetFilePath);
            }

            File.Delete(e.FullPath);
            File.Delete(targetFilePath!);


        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
