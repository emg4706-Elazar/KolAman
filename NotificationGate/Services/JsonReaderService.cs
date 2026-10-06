using System.Text.Json;
using NotificationGate.Models;

namespace NotificationGate.Services;

public class JsonReaderService
{
    public Alert LoadJson(string filepath)
    {

        try
        {
            string text = File.ReadAllText(filepath);
            var alerts = JsonSerializer.Deserialize<Alert>(text);

            return alerts ?? new Alert();
        }
        catch (JsonException e)
        {
            Console.WriteLine($"Failed: {e.Message}");
            return new Alert();
        }  
    }

    
}
