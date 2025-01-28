using System.Text.Json;

namespace Bookery.Data;

public class Deserializer
{
    private JsonSerializerOptions _options;

    public Deserializer()
    {
        _options = new()
        {
            Converters = { new DateOnlyJsonConverter() }
        };
    }

    public T? Deserialize<T>(string s)
    {
        return JsonSerializer.Deserialize<T>(s, _options);
    }
}

