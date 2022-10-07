namespace DisplaySettings.Actions
{
    using System;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using DisplaySettings.Serialization;
    using DisplaySettings.Services;
    using StreamDeck.Events;

    /// <summary>
    /// Provides an action capable of setting the resolution of a display.
    /// </summary>
    [Action(
        Name = "Resolution",
        Icon = "Images/SetDisplayResolution/Icon",
        StateImage = "Images/SetDisplayResolution/Icon",
        PropertyInspectorPath = "pi/set-display-resolution.html")]
    public class SetDisplayResolution : StreamDeckAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetDisplayResolution"/> class.
        /// </summary>
        /// <param name="context">The action's initialization context.</param>
        public SetDisplayResolution(ActionInitializationContext context, DisplayService displayService)
            : base(context) => this.DisplayService = displayService;

        /// <summary>
        /// Gets the display service.
        /// </summary>
        private DisplayService DisplayService { get; }

        /// <inheritdoc/>
        protected override async Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            if (args.Payload.GetSettings<Settings>() is { DeviceName: not null, Resolution: not null } settings
                && this.DisplayService.SetResolution(settings.DeviceName, settings.Resolution.Value))
            {
                await this.ShowOkAsync();
            }
            else
            {
                await this.ShowAlertAsync();
            }
        }

        /// <summary>
        /// Provides settings for the <see cref="SetDisplayResolution"/>.
        /// </summary>
        public record Settings(string? DeviceName, [property: JsonConverter(typeof(ResolutionConverter))] Resolution? Resolution);
    }
}
