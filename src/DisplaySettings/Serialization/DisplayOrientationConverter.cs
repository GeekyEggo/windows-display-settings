namespace DisplaySettings.Serialization
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using WindowsDisplayAPI.Native.DeviceContext;

    /// <summary>
    /// Provides a <see cref="JsonConverter"/> that converts to/from <see cref="DisplayOrientation"/>.
    /// </summary>
    internal class DisplayOrientationConverter : JsonConverter<DisplayOrientation?>
    {
        /// <inheritdoc/>
        public override DisplayOrientation? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var strValue = reader.GetString();
            if (strValue is not null
                && int.TryParse(strValue, out var value))
            {
                return (DisplayOrientation)value;
            }

            return null;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, DisplayOrientation? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(((int)value).ToString());
            }
        }
    }
}
