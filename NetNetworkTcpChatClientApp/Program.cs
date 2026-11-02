using System.Net;
using System.Net.Sockets;

int port = 5000;
string? serverAddress = string.Empty;

Console.Write("Input server address: ");
serverAddress = Console.ReadLine(); 

IPAddress serverHost = String.IsNullOrEmpty(serverAddress) ?
                             IPAddress.Loopback : IPAddress.Parse(serverAddress);

using TcpClient client = new();

StreamReader? reader;
StreamWriter? writer;

try
{
    client.Connect(serverHost, port);

    reader = new(client.GetStream());
    writer = new(client.GetStream());

    if (reader is null || writer is null) return;

    Task.Run(() => ReceiveMessageAsync(reader));

    await SendMessageAsync(writer);
}
catch(Exception ex)
{

}


async Task SendMessageAsync(StreamWriter writer)
{
    Console.WriteLine("Input message and <Enter>");
    Console.WriteLine("-name <name>: send name to server");
    Console.WriteLine("-color <color>: send color to server");

    string? message;

    while(true)
    {
        message = Console.ReadLine();
        await writer.WriteLineAsync(message);
        await writer.FlushAsync();
    }
}

async Task ReceiveMessageAsync(StreamReader reader)
{
    string? message;
    while(true)
    {
        try
        {
            message = await reader.ReadLineAsync();
            if(String.IsNullOrEmpty(message)) continue;

            PrintLine(message);
        }
        catch
        {
            break;
        }
    }
}

void PrintLine(string message)
{
    if(OperatingSystem.IsWindows())
    {
        var position = Console.GetCursorPosition();

        Console.MoveBufferArea(0, position.Top, position.Left, 1, 0, position.Top + 1);
        Console.SetCursorPosition(0, position.Top);
        Console.WriteLine(message);
        Console.SetCursorPosition(position.Left, position.Top + 1);
    }
    else
    {
        Console.WriteLine(message);
    }
    
}