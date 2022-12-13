namespace DisplaySettings.Serialization
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using DisplaySettings.Services;

    /// <summary>
    /// Provides a <see cref="JsonConverter{T}"/> for <see cref="Resolution"/>.
    /// </summary>
    internal class ResolutionConverter : JsonConverter<Resolution?>
    {
        /// <inheritdoc/>
        public override Resolution? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => Resolution.TryCreate(reader.GetString(), out var result) ? result : null;

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, Resolution? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }
}
