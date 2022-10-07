namespace DisplaySettings.Actions
{
    using System;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using DisplaySettings.Serialization;
    using DisplaySettings.Services;
    using StreamDeck.Events;
    using WindowsDisplayAPI.Native.DisplayConfig;

    /// <summary>
    /// Provides an action capable of setting the display configuration, i.e. "Extend", "Clone" etc.
    /// </summary>
    [Action(
        Name = "Project",
        Icon = "Images/Project/Icon",
        StateImage = "Images/Project/Icon",
        PropertyInspectorPath = "pi/project.html")]
    public class Project : StreamDeckAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="displayService">The display service.</param>
        public Project(ActionInitializationContext context, DisplayService displayService)
            : base(context) => this.DisplayService = displayService;

        /// <summary>
        /// Gets the display service.
        /// </summary>
        private DisplayService DisplayService { get; }

        /// <inheritdoc/>
        protected override async Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            if (args.Payload.GetSettings<Settings>() is { Project: not null } settings)
            {
                this.DisplayService.SetDisplayConfig(settings.Project.Value);
                await this.ShowOkAsync();
            }
            else
            {
                await this.ShowAlertAsync();
            }
        }

        /// <summary>
        /// Provides settings for the <see cref="Actions.Project"/>.
        /// </summary>
        public record Settings([property: JsonConverter(typeof(DisplayConfigTopologyIdConverter))] DisplayConfigTopologyId? Project);
    }
}
