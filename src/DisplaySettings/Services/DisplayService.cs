namespace DisplaySettings.Services
{
    using System.Collections.Generic;
    using System.Management;
    using System.Runtime.Versioning;
    using DisplaySettings.Extensions;
    using StreamDeck.Extensions.PropertyInspectors;
    using WindowsDisplayAPI;
    using WindowsDisplayAPI.DisplayConfig;
    using WindowsDisplayAPI.Native.DeviceContext;
    using WindowsDisplayAPI.Native.DisplayConfig;

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
        public void SetDisplayConfig(DisplayConfigTopologyId value)
        {
            if (value is not DisplayConfigTopologyId.Clone
                and not DisplayConfigTopologyId.Extend
                and not DisplayConfigTopologyId.External
                and not DisplayConfigTopologyId.Internal)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "The prefered projection is not a valid option.");
            }

            PathInfo.ApplyTopology(value, true);
        }

        /// <summary>
        /// Sets the <paramref name="orientation"/> of the display.
        /// </summary>
        /// <param name="deviceName">Name of the display device.</param>
        /// <param name="orientation">The orientation.</param>
        /// <returns><c>true</c> when the orientation was successfully set; otherwise <c>false</c>.</returns>
        public bool SetOrientation(string deviceName, DisplayOrientation orientation)
        {
            if (Display.GetDisplays().FirstOrDefault(d => d.DisplayName == deviceName) is Display display and not null)
            {
                display.SetSettings(orientation: orientation);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the <paramref name="resolution"/> of the display.
        /// </summary>
        /// <param name="deviceName">Name of the display device.</param>
        /// <param name="resolution">The resolution.</param>
        /// <returns><c>true</c> when the resolution was successfully set; otherwise <c>false</c>.</returns>
        public bool SetResolution(string deviceName, Resolution resolution)
        {
            if (Display.GetDisplays().FirstOrDefault(d => d.DisplayName == deviceName) is Display display and not null)
            {
                display.SetSettings(resolution);
                return true;
            }

            return false;
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
