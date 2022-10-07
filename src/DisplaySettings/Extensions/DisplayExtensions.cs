namespace DisplaySettings.Extensions
{
    using System.Drawing;
    using WindowsDisplayAPI;
    using WindowsDisplayAPI.Native.DeviceContext;

    /// <summary>
    /// Provides extension methods for <see cref="Display"/>.
    /// </summary>
    public static class DisplayExtensions
    {
        /// <summary>
        /// Sets the <see cref="DisplaySetting"/> for this instance.
        /// </summary>
        /// <param name="display">The <see cref="Display"/> whose settings are being updated.</param>
        /// <param name="resolution">The preferred resolution.</param>
        /// <param name="orientation">The preferred orientation.</param>
        public static void SetSettings(this Display display, Size? resolution = null, DisplayOrientation? orientation = null)
        {
            // Load the current settings once.
            var settings = display.CurrentSetting;

            // Determine the orientation, and then the correct width and height.
            orientation ??= settings.Orientation;
            if (resolution is not null && orientation is DisplayOrientation.Rotate90Degree or DisplayOrientation.Rotate270Degree)
            {
                resolution = new Size(resolution.Value.Height, resolution.Value.Width);
            }
            else if ((int)orientation % 2 != (int)settings.Orientation % 2)
            {
                resolution = new Size(settings.Resolution.Height, settings.Resolution.Width);
            }

            // Create the new settings, and apply them.
            settings = new DisplaySetting(
                resolution is not null ? resolution.Value : settings.Resolution,
                settings.Position,
                settings.ColorDepth,
                settings.Frequency,
                settings.IsInterlaced,
                orientation is not null ? orientation.Value : settings.Orientation,
                settings.OutputScalingMode);

            display.SetSettings(settings, true);
        }
    }
}
