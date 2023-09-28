namespace Commands;

public class CommandHandler
{
    private readonly IStorage _storage;

    public CommandHandler(IStorage storage)
    {
        this._storage = storage;
    }
    public int Handle(StreamWriter sw, object? command)
    {
        if (command == null) return 0;

        if (command is object[] commands)
        {
            var commandResponse = ((string)commands[0]).ToLower() switch
            {
                "echo" => $"+{((string)commands[1])}\r\n",
                "ping" => "+PONG\r\n",
                "set" => _storage.Set((string)commands[1], commands[2], commands.ElementAtOrDefault(3), commands.ElementAtOrDefault(4)),
                "get" => _storage.Get((string)commands[1]),
                _ => string.Empty,
            };
            sw.Write(commandResponse);
            return 0;
        }
        if (int.TryParse((string)command, out var number))
        {
            return -1;
        }

        var response = ((string)command).ToLower() switch
        {
            "ping" => "+PONG\r\n",
            _ => string.Empty,
        };
        sw.Write(response);
        return 0;
    }
}


