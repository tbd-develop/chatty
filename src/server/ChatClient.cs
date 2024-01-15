using System.Net.Sockets;
using System.Text;

namespace server;

public class ChatClient(
    ChatServer server,
    TcpClient client) : IDisposable
{
    private StreamReader _reader = new(client.GetStream(), Encoding.UTF8);
    private StreamWriter _writer = new(client.GetStream(), Encoding.UTF8);

    public async Task Listen(CancellationToken cancellationToken)
    {
        while (client.Connected)
        {
            string message = (await _reader.ReadLineAsync(cancellationToken))!;
            await server.Broadcast(this, message);
        }
        Console.WriteLine("Closed connection");
    }

    public async Task Send(string message)
    {
        await _writer.WriteLineAsync(message);
        _writer.Flush();
    }

    public void Dispose()
    {
        _writer.Dispose();
        _reader.Dispose();
        client.Dispose();
    }
}