namespace DisplaySettings.Actions
{
    using System;
    using System.Threading.Tasks;
    using DisplaySettings.Extensions;
    using DisplaySettings.Services;

    /// <summary>
    /// Sets the advanced color (HDR) state of a display.
    /// </summary>
    /// <seealso cref="StreamDeck.StreamDeckAction" />
    [Action(
        Name = "Use HDR",
        Icon = "Images/SetAdvancedColor/Icon",
        StateImage = "Images/SetAdvancedColor/Icon",
        PropertyInspectorPath = "pi/set-advanced-color.html",
        SortIndex = 4)]
    public class SetAdvancedColor : StreamDeckAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetAdvancedColor"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public SetAdvancedColor(ActionInitializationContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override async Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
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
