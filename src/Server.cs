using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;

TcpListener server = new(IPAddress.Any, 6379);
server.Start();

using Socket socket = server.AcceptSocket();
socket.Send(Server.Serialize(Server.pong), SocketFlags.None);

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
