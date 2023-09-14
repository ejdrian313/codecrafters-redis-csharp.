using System.Net;
using System.Net.Sockets;
using System.Text;

TcpListener server = new(IPAddress.Any, 6379);
server.Start();

byte[] bytes = new byte[256];
while (true)
{
    using TcpClient client = server.AcceptTcpClient();
    ThreadPool.QueueUserWorkItem(HandleClient, client);
}

void HandleClient(object? state)
{
    if (state is not TcpClient) return;
    TcpClient client = (TcpClient)state;
    NetworkStream clientStream = client.GetStream();
    while (clientStream.Read(bytes, 0, bytes.Length) != 0)
    {
        Span<byte> msg = Encoding.ASCII.GetBytes("+PONG\r\n");
        clientStream.Write(msg);
    }
}

// Redis redis = new(new(IPAddress.Loopback, 6379));
// redis.Start();
// using (Socket socket = redis.AcceptSocket())
// {
//     while (true)
//     {
//         redis.HandleMessage(socket);
//     }
// }


// class Redis
// {
//     private readonly TcpListener server;
//     private readonly byte[] data = new byte[1024];

//     public Redis(TcpListener server) => this.server = server;

//     public void Start()
//     {
//         server.Start();
//         Console.WriteLine("Server has started on {0}.", server.LocalEndpoint);
//     }

//     internal void HandleMessage(Socket? socket)
//     {
//         if (socket == null) return;
//         var message = GetMessage(socket);
//         // if (IsPing(message))
//         {
//             PongResposne(socket, message!);
//         }
//     }

//     static bool IsPing(string? message)
//     {
//         Console.WriteLine($"IsPing: {message?.StartsWith(Message.ping) == true}");

//         return message?.StartsWith(Message.ping) == true;
//     }

//     private string? GetMessage(Socket socket)
//     {
//         if (socket.Available == 0) return null;
//         var receivedBytes = socket.Receive(data);
//         if (receivedBytes == 0) return null;

//         var receivedMessaged = Encoding.ASCII.GetString(data, 0, receivedBytes);
//         Console.WriteLine($"receivedMessaged: {receivedMessaged}");
//         return receivedMessaged;
//     }

//     private void PongResposne(Socket socket, string recivedMessage)
//     {
//         // var response = "+PONG\r\n";
//         // var elapsedMessage = recivedMessage.Replace(Message.ping, string.Empty);
//         // Console.WriteLine($"elapsedMessage: {elapsedMessage}");
//         // if (elapsedMessage?.Length > 0)
//         // {
//         //     response = elapsedMessage;
//         // }
//         // Console.WriteLine(response);
//         socket.Send(Message.Serialize(Message.pong), SocketFlags.None);
//     }

//     internal Socket AcceptSocket()
//     {
//         return server.AcceptSocket();
//     }
// }

// class Message
// {
//     public const string pong = "PONG";
//     public const string ping = "ping";
//     public const string endMessage = "\r\n";
//     public const string stringType = "+";
//     public static byte[] Serialize(string value)
//     {
//         return Encoding.UTF8.GetBytes($"{stringType}{value}{endMessage}");
//     }
// }
