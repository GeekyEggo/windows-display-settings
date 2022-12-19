namespace DisplaySettings.Services
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using DisplaySettings.Extensions;
    using Microsoft.Win32;
    using StreamDeck.Extensions.PropertyInspectors;
    using WindowsDisplayAPI;

    /// <summary>
    /// Provides methods and interaction for displays.
    /// </summary>
    public sealed class DisplayService : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayService"/> class.
        /// </summary>
        public DisplayService()
            => SystemEvents.DisplaySettingsChanged += this.OnDisplaySettingsChanged;

        /// <summary>
        /// Occurs when display settings have changed.
        /// </summary>
        public event EventHandler? DisplaySettingsChanged;

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
        /// <param name="displayName">The display name.</param>
        /// <returns>The resolutions.</returns>
        public static IEnumerable<DataSourceItem> GetResolutions(string displayName)
        {
            if (TryGetDisplay(displayName, out var display))
            {
                return display.GetPossibleSettings()
                    .Select(s => new Resolution(s.Resolution.Width, s.Resolution.Height, display.CurrentSetting.Orientation))
                    .Distinct()
                    .OrderByDescending(r => r.Height)
                    .ThenByDescending(r => r.Width)
                    .Select(r => r.ToDataSourceItem());
            }
            else
            {
                return new[] { new DataSourceItem(string.Empty, "No resolutions found", disabled: true) };
            }
        }

        /// <summary>
        /// Gets the display that matches the <paramref name="displayName"/>
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="value">The display.</param>
        /// <returns><c>true</c> when the display was found; otherwise <c>false</c>.</returns>
        public static bool TryGetDisplay(string? displayName, [NotNullWhen(true)] out Display? value)
        {
            if (displayName is not null
                && Display.GetDisplays().FirstOrDefault(d => d.DisplayName == displayName) is Display display and not null)
            {
                value = display;
                return true;
            }

            value = default;
            return false;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            SystemEvents.DisplaySettingsChanged -= this.OnDisplaySettingsChanged;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Converts the <see cref="Display"/> to a <see cref="DataSourceItem"/>
        /// </summary>
        /// <param name="display">The <see cref="Display"/>.</param>
        /// <returns>The converted <see cref="DataSourceItem"/>.</returns>
        private static DataSourceItem ToDataSourceItem(Display display)
            => new DataSourceItem(display.DisplayName, display.ToPathDisplayTarget().FriendlyName, disabled: !display.IsAvailable);

        /// <summary>
        /// Propagate <see cref="SystemEvents.DisplaySettingsChanged"/> to <see cref="DisplaySettingsChanged"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnDisplaySettingsChanged(object? sender, EventArgs e)
            => this.DisplaySettingsChanged?.Invoke(sender, e);
    }
}
