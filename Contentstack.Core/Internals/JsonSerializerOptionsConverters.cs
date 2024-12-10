using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

public class CustomUtcDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            // Parse the string, treating it as UTC if no timezone is specified
            if (DateTime.TryParse(reader.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime dateTime))
            {
                return dateTime.Kind == DateTimeKind.Unspecified 
                    ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc) 
                    : dateTime;
            }
        }
        
        return reader.GetDateTime();
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // Ensure the datetime is in UTC when writing
        writer.WriteStringValue(value.Kind == DateTimeKind.Local 
            ? value.ToUniversalTime() 
            : value);
    }
}

public class CustomNullableUtcDateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.String)
        {
            // Parse the string, treating it as UTC if no timezone is specified
            if (DateTime.TryParse(reader.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime dateTime))
            {
                return dateTime.Kind == DateTimeKind.Unspecified 
                    ? DateTime.SpecifyKind(dateTime, DateTimeKind.Utc) 
                    : dateTime;
            }
        }
        
        return reader.GetDateTime();
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            // Ensure the datetime is in UTC when writing
            writer.WriteStringValue(value.Value.Kind == DateTimeKind.Local 
                ? value.Value.ToUniversalTime() 
                : value.Value);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}