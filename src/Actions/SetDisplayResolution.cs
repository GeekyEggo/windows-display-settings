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
        SortIndex = 2)]
    public class SetDisplayResolution : StreamDeckAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetDisplayResolution"/> class.
        /// </summary>
        /// <param name="context">The action's initialization context.</param>
        public SetDisplayResolution(ActionInitializationContext context)
            : base(context) => this.DisplayName = context.ActionInfo.Payload.GetSettings<Settings>()?.DisplayName;

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        private string? DisplayName { get; set; }

        /// <inheritdoc/>
        protected override async Task OnDidReceiveSettings(ActionEventArgs<ActionPayload> args)
        {
            await base.OnDidReceiveSettings(args);
            if (args.Payload.GetSettings<Settings>() is Settings settings and not null
                && this.DisplayName != settings.DisplayName)
            {
                this.DisplayName = settings.DisplayName;
                await this.SendResolutionsToPropertyInspectorAsync();
            }
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

        /// <inheritdoc/>
        protected override async Task OnPropertyInspectorDidAppear(ActionEventArgs args)
        {
            await base.OnPropertyInspectorDidAppear(args);
            await this.SendResolutionsToPropertyInspectorAsync();
        }

        /// <summary>
        /// Gets the resolutions, and sends them to the property inspector asynchronously.
        /// </summary>
        /// <returns>The task of getting the resolutions.</returns>
        private async Task SendResolutionsToPropertyInspectorAsync()
        {
            const string eventName = "getResolutions";
            if (this.DisplayName is not null)
            {
                await this.SendDataSourceToPropertyInspectorAsync(eventName, DisplayService.GetResolutions(this.DisplayName));
            }
        }

        /// <summary>
        /// Provides settings for the <see cref="SetDisplayResolution"/>.
        /// </summary>
        public record Settings(string? DisplayName, [property: JsonConverter(typeof(ResolutionConverter))] Resolution? Resolution);
    }
}
