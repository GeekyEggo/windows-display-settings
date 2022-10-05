namespace MonitorManager.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Provides information about a display resolution.
    /// </summary>
    public struct Resolution
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Resolution"/> struct.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Resolution(int width, int height)
        {
            this.Height = height;
            this.Width = width;
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
        /// Gets the width.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public int Height { get; }

        /// <inheritdoc/>
        public override int GetHashCode()
            => HashCode.Combine(this.Width, this.Height);

        /// <inheritdoc/>
        public override string ToString()
            => $"{this.Width} x {this.Height}";

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object? obj)
            => obj is Resolution other && other.Width == this.Width && other.Height == this.Height;
    }
}
