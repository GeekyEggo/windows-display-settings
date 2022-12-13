namespace DisplaySettings.Services
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Management;
    using System.Runtime.Versioning;
    using DisplaySettings.Extensions;
    using StreamDeck.Extensions.PropertyInspectors;
    using WindowsDisplayAPI;

    /// <summary>
    /// Provides methods and interaction for displays.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class DisplayService
    {
        /// <summary>
        /// Gets the cache containing the resolutions.
        /// </summary>
        private static Lazy<IReadOnlyCollection<Resolution>> Resolutions { get; } = new Lazy<IReadOnlyCollection<Resolution>>(LoadResolutions, true);

        /// <summary>
        /// Gets the available displays.
        /// </summary>
        /// <returns>The displays.</returns>
        public static IEnumerable<DataSourceItem> GetDisplays()
            => Display.GetDisplays().Select(ToDataSourceItem);

        /// <summary>
        /// Gets the displays with HDR support.
        /// </summary>
        /// <returns>The displays.</returns>
        public static IEnumerable<DataSourceItem> GetDisplaysWithHdrSupport()
        {
            var items = Display.GetDisplays()
                .Where(d => d.GetAdvancedColorInfo() is { AdvancedColorSupported: true })
                .Select(ToDataSourceItem)
                .ToArray();

            return items.Length > 0
                ? items
                : new[] { new DataSourceItem(string.Empty, label: "No HDR displays found", disabled: true) };
        }

        /// <summary>
        /// Gets the available resolutions.
        /// </summary>
        /// <returns>The resolutions.</returns>
        public static IEnumerable<DataSourceItem> GetResolutions()
        {
            foreach (var resolution in Resolutions.Value)
            {
                yield return new DataSourceItem(resolution.ToString(), resolution.ToString());
            }
        }

        /// <summary>
        /// Gets the display that matches the <paramref name="displayName"/>
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="value">The display.</param>
        /// <returns><c>true</c> when the display was found; otherwise <c>false</c>.</returns>
        public static bool TryGetDisplay(string displayName, [NotNullWhen(true)] out Display? value)
        {
            if (Display.GetDisplays().FirstOrDefault(d => d.DisplayName == displayName) is Display display and not null)
            {
                value = display;
                return true;
            }

            value = default;
            return false;
        }

        private static DataSourceItem ToDataSourceItem(Display display)
            => new DataSourceItem(display.DisplayName, display.ToPathDisplayTarget().FriendlyName, disabled: !display.IsAvailable);

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
