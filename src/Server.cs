using System.Net;
using System.Net.Sockets;
using System.Text;
using Commands;
namespace Server;

public class Program
{
    private static void Main(string[] args)
    {
        TcpListener server = new(IPAddress.Any, 6379);
        server.Start();
        try
        {
            while (true)
            {
                var newClient = server.AcceptTcpClient();
                Console.WriteLine("Connected! ");

                var t = new Thread(HandleClient);
                Console.WriteLine("Thread! ");

                t.Start(newClient);
                Console.WriteLine("Thread Started ");
            }
        }
        catch (IOException e)
        {
            Console.WriteLine(e.Message);
        }
        Console.WriteLine("Exit!");
    }

    static void HandleClient(object? client)
    {
        Console.WriteLine("HandleClient!");
        if (client is not TcpClient) return;
        Console.WriteLine("client is TCPCLIENT");
        NetworkStream clientStream = ((TcpClient)client).GetStream();
        Console.WriteLine("client stream");

        using var reader = new StreamReader(clientStream);
        Console.WriteLine("reader");
        using var writer = new StreamWriter(clientStream);
        writer.AutoFlush = true;
        Console.WriteLine("writer");
        while (true)
        {
            Console.WriteLine("not empty");
            var result = CommandHandler.Handle(writer, Parser.Parse(reader));
            if (result == -1) break;
        }
        clientStream.Close();
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
