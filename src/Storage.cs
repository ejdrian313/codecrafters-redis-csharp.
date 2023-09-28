namespace Commands;

public interface IStorage
{
    string Get(string key);
    object Set(string key, dynamic value, object? argument, object? expire);
}

public class Storage : IStorage
{
    public Storage() { }

    private readonly Dictionary<string, dynamic> storage = new();
    private readonly Dictionary<string, long> expiredInMilisecond = new();

    public string Get(string key)
    {
        if (IsExpired(key))
        {
            storage.Remove(key);
            expiredInMilisecond.Remove(key);
            return "$-1\r\n";
        }
        storage.TryGetValue(key, out var value);
        return value switch
        {
            string s => $"${s.Length}\r\n{s}\r\n",
            int i => $":{i}\r\n",
            _ => string.Empty,
        };

    }

    private bool IsExpired(string key)
    {
        if (expiredInMilisecond.TryGetValue(key, out var lifetime) && DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() > lifetime)
            return true;
        return false;
    }

    public object Set(string key, dynamic value, object? argument, object? expire = null)
    {
        if (argument?.ToString() == "px" && expire != null)
        {
            _ = long.TryParse(expire.ToString(), out long ms);
            expiredInMilisecond[key] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + ms;
        }
        storage[key] = value;
        return "+OK\r\n";
    }
}
