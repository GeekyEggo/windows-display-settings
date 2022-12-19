namespace DisplaySettings.Actions
{
    using System.Threading.Tasks;
    using DisplaySettings.Extensions;
    using DisplaySettings.Services;
    using WindowsDisplayAPI.Native.DeviceContext;

    /// <summary>
    /// Sets the orientation of a display.
    /// </summary>
    [Action(
        Name = "Orientation",
        Icon = "Images/SetDisplayOrientation/Icon",
        StateImage = "Images/SetDisplayOrientation/Icon",
        PropertyInspectorPath = "pi/set-display-orientation.html",
        SortIndex = 3)]
    public class SetDisplayOrientation : StreamDeckAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetDisplayOrientation"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public SetDisplayOrientation(ActionInitializationContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override async Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            if (args.Payload.GetSettings<Settings>() is { DisplayName: not null, Orientation: not null } settings
                && DisplayService.TryGetDisplay(settings.DisplayName, out var display))
            {
                display.SetSettings(orientation: settings.Orientation.Value);
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
        public record Settings(string? DisplayName, DisplayOrientation? Orientation);
    }
}
