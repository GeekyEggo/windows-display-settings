namespace DisplaySettings.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json.Serialization;
    using WindowsDisplayAPI.Native.DeviceContext;

    /// <summary>
    /// Provides information about a display resolution.
    /// </summary>
    public readonly struct Resolution
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Resolution"/> struct.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        [JsonConstructor]
        public Resolution(int width, int height)
        {
            this.Height = height;
            this.Width = width;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resolution"/> struct.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="orientation">The orientation.</param>
        public Resolution(int width, int height, DisplayOrientation orientation)
        {
            this.Orientation = orientation;
            if (this.Orientation is DisplayOrientation.Identity or DisplayOrientation.Rotate180Degree)
            {
                this.Height = height;
                this.Width = width;
            }
            else
            {
                this.Height = width;
                this.Width = height;
            }
        }

        /// <summary>
        /// Attempts to create a <see cref="Resolution"/> from the <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="result">The result.</param>
        /// <returns><c>true</c> when the <see cref="Resolution"/> was successfully created; otherwise <c>false</c>.</returns>
        public static bool TryCreate(string? value, out Resolution result)
        {
            if (value is null)
            {
                result = default;
                return false;
            }

            var dimensions = value.Split('x');
            if (dimensions.Length is not 2)
            {
                result = default;
                return false;
            }

            return TryCreate(dimensions[0].Trim(), dimensions[1].Trim(), out result);
        }

        /// <summary>
        /// Attempts to create a <see cref="Resolution"/> from the <paramref name="width"/> and <paramref name="height"/>.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="result">The result.</param>
        /// <returns><c>true</c> when the <see cref="Resolution"/> was successfully created; otherwise <c>false</c>.</returns>
        public static bool TryCreate(string? width, string? height, out Resolution result)
        {
            if (int.TryParse(width, out var w)
                && int.TryParse(height, out var h))
            {
                result = new Resolution(w, h);
                return true;
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Resolution"/> to <see cref="System.Drawing.Size"/>.
        /// </summary>
        /// <param name="resolution">The resolution.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator System.Drawing.Size(Resolution resolution)
            => new System.Drawing.Size(resolution.Width, resolution.Height);

        /// <summary>
        /// Gets the width.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the orientation.
        /// </summary>
        private DisplayOrientation Orientation { get; } = DisplayOrientation.Identity;

        /// <summary>
        /// Gets the height.
        /// </summary>
        public int Height { get; }

        /// <inheritdoc/>
        public override int GetHashCode()
            => HashCode.Combine(this.Width, this.Height);

        /// <summary>
        /// Converts this instance to a <see cref="DataSourceItem"/>.
        /// </summary>
        /// <returns>The <see cref="DataSourceItem"/>.</returns>
        public DataSourceItem ToDataSourceItem()
            => this.Orientation is DisplayOrientation.Identity or DisplayOrientation.Rotate180Degree
            ? new DataSourceItem(this.ToString(), this.ToString())
            : new DataSourceItem(this.ToString(), $"{this.Height} x {this.Width}");

        /// <inheritdoc/>
        public override string ToString()
            => $"{this.Width} x {this.Height}";

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object? obj)
            => obj is Resolution other && other.Width == this.Width && other.Height == this.Height;
    }
}
