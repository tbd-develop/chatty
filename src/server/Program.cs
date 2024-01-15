// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Text;

using var cts = new CancellationTokenSource();

var cancellationToken = cts.Token;

Task.Factory.StartNew(async () =>
{
    using var server = new ChatServer(IPAddress.Any, 7007);

    await server.Start(cancellationToken);
}, cancellationToken);

Console.WriteLine("Press any key to exit...");

Console.ReadKey();

cts.Cancel();