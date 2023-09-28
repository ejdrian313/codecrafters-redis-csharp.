namespace Commands;

public static class CommandHandler
{
    public static int Handle(StreamWriter sw, object? command)
    {
        Console.WriteLine("Command: {0}", command);
        if (command == null) return 0;

        if (command is object[] commands)
        {
            var commandResponse = ((string)commands[0]).ToLower() switch
            {
                "echo" => $"+{((string)commands[1])}\r\n",
                "ping" => "+PONG\r\n",
                _ => string.Empty,
            };
            Console.WriteLine("commandResponse: {0}", commandResponse);
            sw.Write(commandResponse);
            return 0;
        }
        if (int.TryParse((string)command, out var number))
        {
            Console.WriteLine("number: {0}", number);
            return -1;
        }

        var response = ((string)command).ToLower() switch
        {
            "ping" => "+PONG\r\n",
            _ => string.Empty,
        };
        Console.WriteLine("Response: {0}", response);
        sw.Write(response);
        return 0;
    }
}

