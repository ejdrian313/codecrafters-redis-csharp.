using System.Net;
using System.Net.Sockets;
using System.Text;

Redis redis = new(new(IPAddress.Loopback, 6379));
redis.Start();
while (true)
{
    redis.HandleMessage();
}

class Redis
{
    private readonly TcpListener server;
    private readonly byte[] data = new byte[1024];

    public Redis(TcpListener server) => this.server = server;

    public void Start()
    {
        server.Start();
        Console.WriteLine("Server has started on {0}.", server.LocalEndpoint);
    }

    internal void HandleMessage()
    {
        var socket = server.AcceptSocket();
        var message = GetMessage(socket);
        if (IsPing(message))
        {
            PongResposne(socket, message!);
        }
    }

    static bool IsPing(string? message)
    {
        return message?.StartsWith(Server.ping) == true;
    }

    private string? GetMessage(Socket socket)
    {
        if (socket.Available == 0) return null;
        var receivedBytes = socket.Receive(data);
        if (receivedBytes == 0) return null;

        var receivedMessaged = Encoding.ASCII.GetString(data, 0, receivedBytes);
        Console.WriteLine(receivedMessaged);
        return receivedMessaged;
    }

    private void PongResposne(Socket socket, string recivedMessage)
    {
        var response = "+PONG\r\n";
        var elapsedMessage = recivedMessage.Replace(Message.ping, string.Empty);
        if (elapsedMessage?.Length > 0)
        {
            response = elapsedMessage;
        }
        Console.WriteLine(response);
        socket.Send(Message.Serialize(response), SocketFlags.None);
    }
}

class Message
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
