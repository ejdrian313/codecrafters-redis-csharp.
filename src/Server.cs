using System.Net;
using System.Net.Sockets;
using System.Text;

TcpListener server = new(IPAddress.Any, 6379);
server.Start();

using Socket socket = server.AcceptSocket();
socket.Receive(new byte[1024]);

var data = new byte[1024];
while (true)
{
    if (socket.Available == 0) continue;
    var receivedBytes = socket.Receive(data);
    if (receivedBytes == 0) continue;

    var receivedMessaged = Encoding.ASCII.GetString(data, 0, receivedBytes);
    Console.WriteLine(receivedMessaged);

    if (receivedMessaged == Server.ping)
    {
        socket.Send(Server.Serialize(Server.pong), SocketFlags.None);
    }
}


class Server
{
    public const string pong = "PONG";
    public const string ping = "PING";
    public const string endMessage = "\r\n";
    public const string stringType = "+";
    public static byte[] Serialize(string value)
    {
        return Encoding.UTF8.GetBytes($"{stringType}{value}{endMessage}");
    }
}
