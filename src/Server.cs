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
                var t = new Thread(HandleClient);
                t.Start(newClient);
            }
        }
        catch (IOException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    static void HandleClient(object? client)
    {

        if (client is not TcpClient) return;
        NetworkStream clientStream = ((TcpClient)client).GetStream();

        using var reader = new StreamReader(clientStream);
        using var writer = new StreamWriter(clientStream);
        writer.AutoFlush = true;
        CommandHandler commandHandler = new(new Storage());
        while (true)
        {
            var result = commandHandler.Handle(writer, Parser.Parse(reader));
            if (result == -1) break;
        }
        clientStream.Close();
    }
}
