namespace DisplaySettings.Serialization
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using WindowsDisplayAPI.Native.DisplayConfig;

    /// <summary>
    /// Provides a <see cref="JsonConverter"/> that converts to/from <see cref="DisplayConfigTopologyId"/>.
    /// </summary>
    internal class DisplayConfigTopologyIdConverter : JsonConverter<DisplayConfigTopologyId?>
    {
        /// <inheritdoc/>
        public override DisplayConfigTopologyId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var strValue = reader.GetString();
            if (strValue is not null
                && int.TryParse(strValue, out var value))
            {
                return (DisplayConfigTopologyId)value;
            }

            return null;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, DisplayConfigTopologyId? value, JsonSerializerOptions options)
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
