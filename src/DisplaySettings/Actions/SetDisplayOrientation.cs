namespace DisplaySettings.Actions
{
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using DisplaySettings.Serialization;
    using DisplaySettings.Services;
    using StreamDeck.Events;
    using WindowsDisplayAPI.Native.DeviceContext;

    [Action(
        Name = "Orientation",
        Icon = "Images/SetDisplayOrientation/Icon",
        StateImage = "Images/SetDisplayOrientation/Icon",
        PropertyInspectorPath = "pi/set-display-orientation.html")]
    public class SetDisplayOrientation : StreamDeckAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetDisplayOrientation"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="displayService">The display service.</param>
        public SetDisplayOrientation(ActionInitializationContext context, DisplayService displayService)
            : base(context) => this.DisplayService = displayService;

        /// <summary>
        /// Gets the display service.
        /// </summary>
        private DisplayService DisplayService { get; }

        /// <inheritdoc/>
        protected override async Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            if (args.Payload.GetSettings<Settings>() is { DeviceName: not null, Orientation: not null } settings
                && this.DisplayService.SetOrientation(settings.DeviceName, settings.Orientation.Value))
            {
                await this.ShowOkAsync();
            }
            else
            {
                await this.ShowAlertAsync();
            }
        }

        /// <summary>
        /// Provides settings for the <see cref="SetDisplayOrientation"/>.
        /// </summary>
        public record Settings(string? DeviceName, [property: JsonConverter(typeof(DisplayOrientationConverter))] DisplayOrientation? Orientation);
    }
}
