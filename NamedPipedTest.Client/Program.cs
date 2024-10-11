using System.IO.Pipes;

namespace NamedPipedTest.Client;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Healthcare client is running...");

        Console.WriteLine("Enter the path of file to send:");
        var filePath = Console.ReadLine();

        if (!File.Exists(filePath))
        {
            Console.WriteLine("File not found :(");
            return;
        }

        using (var pipeClient = new NamedPipeClientStream(".", "HealthCarePipe", PipeDirection.InOut))
        {
            try
            {
                Console.WriteLine("Connecting to server...");
                pipeClient.Connect();

                SendFile(pipeClient, filePath);
                Console.WriteLine("File sent to server.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }

        Console.WriteLine("Press enter to exit.");
        Console.ReadLine();
    }

    static void SendFile(NamedPipeClientStream pipeClient, string filePath)
    {
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var buffer = new byte[4096];
        int bytesRead;

        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            pipeClient.Write(buffer, 0, bytesRead);
        }
    }
}