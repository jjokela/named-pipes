using System.IO.Pipes;

namespace NamedPipedTest.Server;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Healthcare server is running...");
        StartServerAsync();
        Console.WriteLine("Press enter to exit");
        Console.ReadLine();
    }

    private static async void StartServerAsync()
    {
        await Task.Run(StartServer);
    }

    private static Task StartServer()
    {
        while (true)
        {
            using var pipeServer = new NamedPipeServerStream("HealthcarePipe", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            Console.WriteLine("Waiting for client connection...");
            pipeServer.WaitForConnection();
            Console.WriteLine("Client connected!");

            try
            {
                ReceiveFile(pipeServer);
                Console.WriteLine("File received and saved.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }
    }

    static void ReceiveFile(NamedPipeServerStream pipeServer)
    {
        var outputFile = "ReceivedFile.dat";

        using var fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
        var buffer = new byte[4096];
        int bytesRead;

        while ((bytesRead = pipeServer.Read(buffer, 0, buffer.Length)) > 0)
        {
            fileStream.Write(buffer, 0, bytesRead);
        }
    }
}