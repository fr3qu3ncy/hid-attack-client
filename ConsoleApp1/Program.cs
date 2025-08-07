using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

class Program
{
    static async Task Main()
    {
        var url = "https://n8n.lan/webhook/7994dc88-9513-4cf3-8717-a32a1db60704";
        using var client = new HttpClient();

        // Store the conversation history as a list of strings
        var history = new List<string>();

        // Silently send initial message and display the bot's response
        string initialMessage = "What is going on? Who Are You?";
        var initialPayload = new { data = initialMessage };
        var initialJson = JsonSerializer.Serialize(initialPayload);
        var initialContent = new StringContent(initialJson, Encoding.UTF8, "application/json");

        Console.WriteLine("Connecting to Hacker Net..........");
        Console.WriteLine(" ");
        Console.WriteLine("Simple Hacker Chat Client.");
        Console.WriteLine("..........");
        Console.WriteLine(" ");

        try
        {
            using var initialResponse = await client.PostAsync(url, initialContent);
            initialResponse.EnsureSuccessStatusCode();

            var initialResponseStream = await initialResponse.Content.ReadAsStreamAsync();
            var initialResponseJson = await JsonSerializer.DeserializeAsync<JsonElement>(initialResponseStream);

            if (initialResponseJson.TryGetProperty("text", out var initialTextProp))
            {
                string botReply = initialTextProp.GetString();
                Console.WriteLine($"Hacker: {botReply}");
                history.Add($"You: {initialMessage}");
                history.Add($"Hacker: {botReply}");
            }
            else
            {
                Console.WriteLine("Hacker: (No 'text' property in response)");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

         Console.WriteLine("--------------------------------------------------");
        Console.WriteLine(" ");
        Console.WriteLine("Simple Hacker Chat Client. Type your message and press Enter.");

        while (true)
        {
            Console.Write("\nYou: ");
            string userMessage = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(userMessage) || userMessage.Trim().ToLower() == "exit")
                break;

            // Build the message history string
            var fullMessage = string.Join("\n", history);
            if (!string.IsNullOrEmpty(fullMessage))
                fullMessage += "\n";
            fullMessage += $"You: {userMessage}";

            var payload = new { data = fullMessage };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                using var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var responseStream = await response.Content.ReadAsStreamAsync();
                var responseJson = await JsonSerializer.DeserializeAsync<JsonElement>(responseStream);

                if (responseJson.TryGetProperty("text", out var textProp))
                {
                    string botReply = textProp.GetString();
                    Console.WriteLine($"Hacker: {botReply}");
                    Console.WriteLine("--------------------------------------------------");

                    // Add user message and bot reply to history
                    history.Add($"You: {userMessage}");
                    history.Add($"Hacker: {botReply}");
                }
                else
                {
                    Console.WriteLine("Hacker: (No 'text' property in response)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        Console.WriteLine("\nGoodbye!");
        try
        {
            Console.ReadLine();
        }
        catch
        {
            // Ignore if no console is available
        }
    }
}
