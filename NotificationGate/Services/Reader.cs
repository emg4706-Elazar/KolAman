using System.Text.Json;
using NotificationGate.Models;

namespace NotificationGate.Services;

public static class Reader
{
    static List<Alert>? ReadFile(string filepath)
    {
        string? file = File.ReadAllText(filepath);

        if (file is null)
            return null;

        try
        {
            List<Alert>? alerts = JsonSerializer
            .Deserialize<List<Alert>>(file);

            return alerts;
        }
        catch (JsonException e)
        {
            Console.WriteLine($"Failed: {e.Message}");
            return null;
        }  
    }
}
