using System.Text;
namespace Commands;

public class Parser
{
    private enum RespType { SimpleString = '+', BulkString = '$', Array = '*', }
    public static object? Parse(StreamReader reader)
    {
        var read = reader.Read();
        if (read == -1) return -1;
        RespType type = (RespType)read;
        return type switch
        {
            RespType.SimpleString => ParseSingleString(reader),
            RespType.BulkString => ParseBulkString(reader),
            RespType.Array => ParseArray(reader),
            _ => null,
        };
    }
    private static string ReadLine(StreamReader reader)
    {
        var sb = new StringBuilder();
        while (true)
        {
            var ch = reader.Read();
            if (ch == '\r')
            {
                reader.Read();
                return sb.ToString();
            }
            sb.Append((char)ch);
        }
    }
    private static string ParseSingleString(StreamReader reader)
    {
        return ReadLine(reader);
    }
    private static string? ParseBulkString(StreamReader reader)
    {
        var len = int.Parse(ReadLine(reader));
        if (len < 0)
            return null;
        else
            return ReadLine(reader);
    }
    private static object[]? ParseArray(StreamReader reader)
    {
        var len = int.Parse(ReadLine(reader));
        if (len < 0)
            return null;
        var values = new object[len];
        for (var i = 0; i < len; i++)
        {
            var parsed = Parse(reader);
            if (parsed != null)
                values[i] = parsed;
        }
        return values;
    }
}
