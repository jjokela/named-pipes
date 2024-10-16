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

    private static async Task StartServer()
    {
        var retryPolicy = ServerRetryPolicy.GetRetryPolicyAsync();

        while (true)
        {
            await retryPolicy.ExecuteAsync(async () =>
            {
                await using var pipeServer = new NamedPipeServerStream("HealthcarePipe", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                Console.WriteLine("Waiting for client connection...");
                try
                {
                    await pipeServer.WaitForConnectionAsync();
                    Console.WriteLine("Client connected!");

                    // Receive the file
                    await ReceiveFileAsync(pipeServer);
                    Console.WriteLine("File received and saved.");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"ERROR: {ex.Message}");
                    throw;
                }
                finally
                {
                    // Ensure the pipe server is disposed properly and client connection is released
                    if (pipeServer.IsConnected)
                    {
                        pipeServer.Disconnect();
                    }
                }
            });
        }
    }


    private static async Task ReceiveFileAsync(NamedPipeServerStream pipeServer)
    {
        var outputFile = "ReceivedFile.dat";

        await using var fileStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
        var buffer = new byte[4096];
        int bytesRead;

        // Read and write asynchronously
        while ((bytesRead = await pipeServer.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            await fileStream.WriteAsync(buffer, 0, bytesRead);
        }
    }
}