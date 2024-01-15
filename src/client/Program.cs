// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;

using var client = new TcpClient();

client.Connect(IPAddress.Loopback, 7007);

bool running = true;

using var stream = client.GetStream();

var writeStream = stream;

Console.Write("Enter your name: ");

string name = Console.ReadLine();

Task.Factory.StartNew(() =>
{
    var data = new Byte[256];

    int bytesRead;

    while ((bytesRead = writeStream.Read(data, 0, data.Length)) > 0)
    {
        var responseData = Encoding.ASCII.GetString(data, 0, bytesRead);

        if (bytesRead >= 256) continue;
        
        Console.WriteLine();
        Console.WriteLine(responseData);
    }
});

while (running)
{
    Console.Write("Send: ");

    string input = Console.ReadLine();

    if (input == "exit")
    {
        running = false;

        continue;
    }

    var data = Encoding.UTF8.GetBytes($"[{name}]:{input}");

    stream.Write(data, 0, data.Length);
}