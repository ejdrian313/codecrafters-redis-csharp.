namespace Commands;

public interface IStorage
{
    dynamic? Get(string key);
    object Set(string key, dynamic value);
}

public class Storage : IStorage
{
    public Storage() { }

    private readonly Dictionary<string, dynamic> storage = new();

    public dynamic? Get(string key) => storage.TryGetValue(key, out var value) ? value : null;

    public object Set(string key, dynamic value)
    {
        // if (storage[key] == value) return value;
        storage[key] = value;
        return "+OK\r\n";
    }
}
