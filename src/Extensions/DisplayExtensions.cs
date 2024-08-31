namespace DisplaySettings.Extensions
{
    using System.Drawing;
    using System.Runtime.InteropServices;
    using Windows.Win32;
    using Windows.Win32.Devices.Display;
    using WindowsDisplayAPI;
    using WindowsDisplayAPI.Native.DeviceContext;

    /// <summary>
    /// Provides extension methods for <see cref="Display"/>.
    /// </summary>
    public static class DisplayExtensions
    {
        /// <summary>
        /// Gets the advanced color information (HDR) for the <see cref="Display"/>.
        /// </summary>
        /// <param name="display">The <see cref="Display"/>.</param>
        /// <returns>The advanced color information.</returns>
        /// <remarks>Thanks to <see href="https://github.com/BartoszCichecki/LenovoLegionToolkit"/>.</remarks>
        public static AdvancedColorInfo GetAdvancedColorInfo(this Display display)
        {
            var getAdvancedColorInfo = new DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO();
            getAdvancedColorInfo.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_ADVANCED_COLOR_INFO;
            getAdvancedColorInfo.header.size = (uint)Marshal.SizeOf(typeof(DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO));
            getAdvancedColorInfo.header.adapterId.HighPart = display.Adapter.ToPathDisplayAdapter().AdapterId.HighPart;
            getAdvancedColorInfo.header.adapterId.LowPart = display.Adapter.ToPathDisplayAdapter().AdapterId.LowPart;
            getAdvancedColorInfo.header.id = display.ToPathDisplayTarget().TargetId;

            if (PInvoke.DisplayConfigGetDeviceInfo(ref getAdvancedColorInfo.header) != 0)
            {
                throw new Exception("Failed to get HDR information.");
            }

            return new(getAdvancedColorInfo.Anonymous.value.GetNthBit(0),
                getAdvancedColorInfo.Anonymous.value.GetNthBit(1),
                getAdvancedColorInfo.Anonymous.value.GetNthBit(2),
                getAdvancedColorInfo.Anonymous.value.GetNthBit(3));
        }

        /// <summary>
        /// Sets the advanced color information (HDR) for the <see cref="Display"/>.
        /// </summary>
        /// <param name="display">The <see cref="Display"/>.</param>
        /// <param name="state">The preferred state of HDR.</param>
        /// <remarks>Thanks to <see href="https://github.com/BartoszCichecki/LenovoLegionToolkit"/>.</remarks>
        public static void SetAdvancedColorState(this Display display, bool state)
        {
            var setAdvancedColorState = new DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE();
            setAdvancedColorState.header.type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_SET_ADVANCED_COLOR_STATE;
            setAdvancedColorState.header.size = (uint)Marshal.SizeOf(typeof(DISPLAYCONFIG_SET_ADVANCED_COLOR_STATE));
            setAdvancedColorState.header.adapterId.HighPart = display.Adapter.ToPathDisplayAdapter().AdapterId.HighPart;
            setAdvancedColorState.header.adapterId.LowPart = display.Adapter.ToPathDisplayAdapter().AdapterId.LowPart;
            setAdvancedColorState.header.id = display.ToPathDisplayTarget().TargetId;

            setAdvancedColorState.Anonymous.value = setAdvancedColorState.Anonymous.value.SetNthBit(0, state);

            if (PInvoke.DisplayConfigSetDeviceInfo(setAdvancedColorState.header) != 0)
            {
                throw new Exception("Failed to set HDR information.");
            }
        }

        /// <summary>
        /// Sets the <see cref="DisplaySetting"/> for this instance.
        /// </summary>
        /// <param name="display">The <see cref="Display"/> whose settings are being updated.</param>
        /// <param name="resolution">The preferred resolution.</param>
        /// <param name="orientation">The preferred orientation.</param>
        /// <param name="scalingMode">The preferred scaling mode.</param>
        public static void SetSettings(this Display display, Size? resolution = null, DisplayOrientation? orientation = null, DisplayFixedOutput scalingMode = DisplayFixedOutput.Default)
        {
            // Load the current settings once.
            var settings = display.CurrentSetting;
            var preferredSetting = display.GetPreferredSetting();

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

            // Native resolution must always have default scaling.
            if (resolution == preferredSetting.Resolution)
            {
                scalingMode = DisplayFixedOutput.Default;
            }

            // Create the new settings, and apply them.
            settings = new DisplaySetting(
                resolution is not null ? resolution.Value : settings.Resolution,
                settings.Position,
                settings.ColorDepth,
                settings.Frequency,
                settings.IsInterlaced,
                orientation is not null ? orientation.Value : settings.Orientation,
                scalingMode);

            display.SetSettings(settings, true);
        }
    }

    /// <summary>
    /// Provides information about the advanced color state of a display.
    /// </summary>
    /// <param name="AdvancedColorSupported">Whether advanced color (HDR) is supported.</param>
    /// <param name="AdvancedColorEnabled">Whether advanced color (HDR) is enabled.</param>
    /// <param name="WideColorEnforced">Whether wide color should be enforced.</param>
    /// <param name="AdvancedColorForceDisabled">Whether advanced color (HDR) should be disabled.</param>
    public readonly record struct AdvancedColorInfo(bool AdvancedColorSupported, bool AdvancedColorEnabled, bool WideColorEnforced, bool AdvancedColorForceDisabled);
}
