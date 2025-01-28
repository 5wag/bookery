using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bookery.Data;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string DateFormat = "yyyyMMdd";

    public override DateOnly Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var dateString = reader.GetString();
        if (string.IsNullOrWhiteSpace(dateString)) return default;
        return DateOnly.ParseExact(dateString!, DateFormat, CultureInfo.InvariantCulture);
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateOnly dateValue,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(dateValue.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}
