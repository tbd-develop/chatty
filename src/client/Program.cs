// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;

using var client = new TcpClient();

client.Connect(IPAddress.Loopback, 7007);

using var reader = new StreamReader(client.GetStream(), Encoding.UTF8);
using var writer = new StreamWriter(client.GetStream(), Encoding.UTF8);

Console.Write("Enter your name: ");

string name = Console.ReadLine();

Task.Factory.StartNew(async () =>
{
    await foreach(string response in ReceiveLines(reader)) {
        Console.WriteLine();
        Console.WriteLine(response);
    }
});

while (true)
{
    Console.Write("Send: ");

    string input = Console.ReadLine();

    if (input == "exit")
        break;

    writer.WriteLine($"[{name}]:{input}");
    writer.Flush();
}

async IAsyncEnumerable<string> ReceiveLines(StreamReader reader)
{
    string? line;
    while ((line = await reader.ReadLineAsync()) != null)
        yield return line;
}