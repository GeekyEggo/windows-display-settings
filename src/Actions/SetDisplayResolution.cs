namespace DisplaySettings.Actions
{
    using System;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using DisplaySettings.Extensions;
    using DisplaySettings.Serialization;
    using DisplaySettings.Services;

    /// <summary>
    /// Provides an action capable of setting the resolution of a display.
    /// </summary>
    [Action(
        Name = "Resolution",
        Icon = "Images/SetDisplayResolution/Icon",
        StateImage = "Images/SetDisplayResolution/Icon",
        PropertyInspectorPath = "pi/set-display-resolution.html",
        SortIndex = 1)]
    public class SetDisplayResolution : StreamDeckAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetDisplayResolution"/> class.
        /// </summary>
        /// <param name="context">The action's initialization context.</param>
        public SetDisplayResolution(ActionInitializationContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override async Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            if (args.Payload.GetSettings<Settings>() is { DisplayName: not null, Resolution: not null } settings
                && DisplayService.TryGetDisplay(settings.DisplayName, out var display))
            {
                display.SetSettings(resolution: settings.Resolution.Value);
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
        public record Settings(string? DisplayName, [property: JsonConverter(typeof(ResolutionConverter))] Resolution? Resolution);
    }
}
