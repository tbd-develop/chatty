using System.Net.Sockets;
using System.Text;

namespace server;

public class ChatClient(
    ChatServer server,
    TcpClient client) : IDisposable
{
    private Stream _stream = null!;

    public async Task Listen(CancellationToken cancellationToken)
    {
        _stream = client.GetStream();

        var buffer = new byte[1024];
        int bytesRead = 0;
        StringBuilder input = new StringBuilder();

        while ((bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
        {
            input.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

            if (bytesRead >= 1024) continue;

            var content = input.ToString();

            if (content.TrimEnd('\r', '\n').Length == 0) continue;

            await server.Broadcast(this, content);

            input.Clear();
        }
    }

    public async Task Send(string message)
    {
        var data = Encoding.UTF8.GetBytes(message);

        await _stream.WriteAsync(data);
    }

    public void Dispose()
    {
        _stream.Dispose();
        client.Dispose();
    }
}