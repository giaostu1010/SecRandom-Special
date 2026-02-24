using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia.Media;

namespace SecRandom.Core.Abstraction;

public class ColorJsonConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value))
            {
                return Colors.White;
            }
            
            try
            {
                return Color.Parse(value);
            }
            catch
            {
                return Colors.White;
            }
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            return Color.FromUInt32(reader.GetUInt32());
        }

        return Colors.White;
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
