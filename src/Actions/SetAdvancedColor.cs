namespace DisplaySettings.Actions
{
    using System;
    using System.Threading.Tasks;
    using DisplaySettings.Extensions;
    using DisplaySettings.Services;

    /// <summary>
    /// Sets the advanced color (HDR) state of a display.
    /// </summary>
    [Action(
        Name = "Use HDR",
        Icon = "Images/SetAdvancedColor/State1",
        PropertyInspectorPath = "pi/set-advanced-color.html",
        DisableAutomaticStates = true,
        SortIndex = 1)]
    [State("Images/SetAdvancedColor/State0")]
    [State("Images/SetAdvancedColor/State1")]
    public class SetAdvancedColor : StreamDeckAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetAdvancedColor"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="displayService">The display service.</param>
        public SetAdvancedColor(ActionInitializationContext context, DisplayService displayService)
            : base(context) => this.DisplayService = displayService;

        /// <summary>
        /// Gets the display service.
        /// </summary>
        private DisplayService DisplayService { get; }

        /// <summary>
        /// Gets or sets the display name associated with this action.
        /// </summary>
        private string? DisplayName { get; set; }

        /// <inheritdoc/>
        protected override async Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args)
        {
            await base.OnDidReceiveSettings(args);

            this.DisplayName = args.Payload.GetSettings<Settings>()?.DisplayName;
            await this.RefreshStateAsync();
        }

        /// <inheritdoc/>
        protected override async Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            await base.OnKeyDown(args);

            if (args.Payload.GetSettings<Settings>() is { DisplayName: not null } settings
                && DisplayService.TryGetDisplay(settings.DisplayName, out var display)
                && display.GetAdvancedColorInfo() is AdvancedColorInfo colorInfo and { AdvancedColorSupported: true })
            {
                if (!colorInfo.AdvancedColorEnabled && settings.Action is State.Toggle or State.On)
                {
                    display.SetAdvancedColorState(true);
                    await this.ShowOkAsync();
                }
                else if (colorInfo.AdvancedColorEnabled && settings.Action is State.Toggle or State.Off)
                {
                    display.SetAdvancedColorState(false);
                    await this.ShowOkAsync();
                }
            }
            else
            {
                await this.ShowAlertAsync();
            }
        }

        /// <inheritdoc/>
        protected override async Task OnWillAppear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillAppear(args);

            this.DisplayName = args.Payload.GetSettings<Settings>()?.DisplayName;
            this.DisplayService.DisplaySettingsChanged += this.DisplayService_DisplaySettingsChanged;
            await this.RefreshStateAsync();
        }

        /// <inheritdoc/>
        protected override async Task OnWillDisappear(ActionEventArgs<AppearancePayload> args)
        {
            await base.OnWillDisappear(args);
            this.DisplayService.DisplaySettingsChanged -= this.DisplayService_DisplaySettingsChanged;
        }

        /// <summary>
        /// Handles the <see cref="DisplayService.DisplaySettingsChanged"/>
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void DisplayService_DisplaySettingsChanged(object? sender, EventArgs e)
            => this.RefreshStateAsync().ConfigureAwait(false);

        /// <summary>
        /// Refreshes the state asynchronously.
        /// </summary>
        /// <returns>The task of refreshing the state.</returns>
        private Task RefreshStateAsync()
        {
            if (DisplayService.TryGetDisplay(this.DisplayName, out var display)
                && display.GetAdvancedColorInfo() is AdvancedColorInfo and { AdvancedColorEnabled: true })
            {
                return this.SetStateAsync(1);
            }
            else
            {
                return this.SetStateAsync(0);
            }
        }

        /// <summary>
        /// Provides settings for the <see cref="SetDisplayResolution"/>.
        /// </summary>
        public record Settings(string? DisplayName, State Action = State.Toggle);

        /// <summary>
        /// An enumeration of possible states.
        /// </summary>
        public enum State
        {
            Toggle = 0,
            On = 1,
            Off = 2
        }
    }
}
