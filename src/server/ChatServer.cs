using System.Net;
using System.Net.Sockets;
using server;

public class ChatServer(IPAddress address, int port) : IDisposable
{
    private readonly List<ChatClient> _clients = new();
    private readonly List<Task> _tasks = new();

    public async Task Start(CancellationToken cancellationToken)
    {
        var server = new TcpListener(address, port);

        server.Start();

        while (!cancellationToken.IsCancellationRequested)
        {
            var acceptedClientConnection = await server.AcceptTcpClientAsync(cancellationToken);

            var client = new ChatClient(this, acceptedClientConnection);

            _clients.Add(client);

            _tasks.Add(client.Listen(cancellationToken));
        }

        await Task.WhenAll(_tasks);
    }

    public async Task Broadcast(ChatClient sender, string message)
    {
        Console.WriteLine($"Broadcasting message: {message}");

        foreach (var client in _clients.Where(client => client != sender))
        {
            await client.Send(message);
        }
    }

    public void Dispose()
    {
        foreach (var client in _clients)
        {
            client.Dispose();
        }
    }
}