namespace DisplaySettings.Services
{
    using System.Collections.Generic;
    using System.Management;
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;
    using DisplaySettings.Interop;
    using StreamDeck.Extensions.PropertyInspectors;
    using WindowsDisplayAPI;

    /// <summary>
    /// Provides methods and interaction for displays.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class DisplayService
    {
        /// <summary>
        /// Gets the cache containing the resolutions.
        /// </summary>
        private static Lazy<IReadOnlyCollection<Resolution>> Resolutions { get; } = new Lazy<IReadOnlyCollection<Resolution>>(LoadResolutions, true);

        /// <summary>
        /// Gets the available displays.
        /// </summary>
        /// <returns>The displays.</returns>
        public IEnumerable<DataSourceItem> GetDisplays()
        {
            foreach (var display in Display.GetDisplays())
            {
                yield return new DataSourceItem(display.DisplayName, display.ToPathDisplayTarget().FriendlyName, disabled: !display.IsAvailable);
            }
        }

        /// <summary>
        /// Gets the available resolutions.
        /// </summary>
        /// <returns>The resolutions.</returns>
        public IEnumerable<DataSourceItem> GetResolutions()
        {
            foreach (var resolution in Resolutions.Value)
            {
                yield return new DataSourceItem(resolution.ToString(), resolution.ToString());
            }
        }

        /// <summary>
        /// Sets the display configuration, i.e. "Clone", "Extend", etc..
        /// </summary>
        /// <param name="value">The desired projection.</param>
        public void SetDisplayConfig(ProjectOption value)
        {
            if (value is not ProjectOption.Clone
                and not ProjectOption.Extend
                and not ProjectOption.External
                and not ProjectOption.Internal)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "The prefered projection is not a valid option.");
            }

            User32.SetDisplayConfig(0, IntPtr.Zero, 0, IntPtr.Zero, (User32.SDC_APPLY | (uint)value));
        }

        /// <summary>
        /// Sets the <paramref name="resolution"/> of the device.
        /// </summary>
        /// <param name="deviceName">Name of the device.</param>
        /// <param name="resolution">The resolution.</param>
        /// <returns><c>true</c> when the resolution was successfully set; otherwise <c>false</c>.</returns>
        public bool SetResolution(string deviceName, Resolution resolution)
        {
            // todo: Add support for rotated monitors.
            var dm = new DEVMODE();
            dm.dmDeviceName = new string(new char[32]);
            dm.dmFormName = new string(new char[32]);
            dm.dmSize = (short)Marshal.SizeOf(dm);

            if (User32.EnumDisplaySettings(deviceName, User32.ENUM_CURRENT_SETTINGS, ref dm) != 0)
            {
                dm.dmPelsWidth = resolution.Width;
                dm.dmPelsHeight = resolution.Height;

                return User32.ChangeDisplaySettings(ref dm, 0) == User32.DISP_CHANGE_SUCCESSFUL;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Loads the available resolutions.
        /// </summary>
        /// <returns>The resolutions, in order.</returns>
        private static IReadOnlyCollection<Resolution> LoadResolutions()
        {
            var resolutions = new HashSet<Resolution>();

            using var searcher = new ManagementObjectSearcher(new ManagementScope(), new ObjectQuery("SELECT * FROM CIM_VideoControllerResolution"));
            foreach (var item in searcher.Get())
            {
                if (Resolution.TryCreate(item["HorizontalResolution"]?.ToString(), item["VerticalResolution"]?.ToString(), out var resolution))
                {
                    resolutions.Add(resolution);
                }
            }

            return resolutions
                .OrderByDescending(r => r.Height)
                .ThenByDescending(r => r.Width)
                .ToArray();
        }
    }
}
