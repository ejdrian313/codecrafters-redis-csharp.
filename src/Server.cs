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
        CommandHandler commandHandler = new(new Storage());
        while (true)
        {
            Console.WriteLine("not empty");
            var result = commandHandler.Handle(writer, Parser.Parse(reader));
            if (result == -1) break;
        }
        clientStream.Close();
    }
}
