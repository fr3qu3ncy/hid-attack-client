using System;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var url = "http://search.lan/";
        using var client = new HttpClient();

        Console.WriteLine($"Requesting: {url}");

        // Send request and get response headers first
        using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var contentLength = response.Content.Headers.ContentLength;
        using var stream = await response.Content.ReadAsStreamAsync();

        const int bufferSize = 8192;
        var buffer = new byte[bufferSize];
        int bytesRead;
        long totalRead = 0;

        Console.Write("Downloading: [");
        int progressBarWidth = 40;
        int lastProgress = 0;

        using var ms = new System.IO.MemoryStream();
        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            ms.Write(buffer, 0, bytesRead);
            totalRead += bytesRead;

            if (contentLength.HasValue)
            {
                int progress = (int)((totalRead * progressBarWidth) / contentLength.Value);
                if (progress != lastProgress)
                {
                    Console.Write(new string('=', progress - lastProgress));
                    lastProgress = progress;
                }
            }
        }

        if (contentLength.HasValue)
        {
            Console.WriteLine(new string('=', progressBarWidth - lastProgress) + "]");
        }
        else
        {
            Console.WriteLine(" done]");
        }

        ms.Position = 0;
        using var reader = new System.IO.StreamReader(ms);
        string content = await reader.ReadToEndAsync();

        Console.WriteLine("\n--- Response ---");
        Console.WriteLine(content);

        Console.WriteLine("\nPress Enter to exit...");
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
