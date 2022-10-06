namespace DisplaySettings.Serialization
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using DisplaySettings.Services;

    /// <summary>
    /// Provides a <see cref="JsonConverter"/> that converts to/from <see cref="ProjectOption"/>.
    /// </summary>
    internal class ProjectOptionConverter : JsonConverter<ProjectOption?>
    {
        /// <inheritdoc/>
        public override ProjectOption? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var strValue = reader.GetString();
            if (strValue is not null
                && int.TryParse(strValue, out var value))
            {
                return (ProjectOption)value;
            }

            return null;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, ProjectOption? value, JsonSerializerOptions options)
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
