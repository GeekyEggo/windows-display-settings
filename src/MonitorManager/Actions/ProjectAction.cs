namespace MonitorManager.Actions
{
    using System;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using MonitorManager.Serialization;
    using MonitorManager.Services;
    using StreamDeck.Events;

    /// <summary>
    /// Provides an action capable of setting the display configuration, i.e. "Extend", "Clone" etc.
    /// </summary>
    [Action(
        Name = "Project",
        Icon = "Images/Project/Icon",
        StateImage = "Images/Project/Icon",
        PropertyInspectorPath = "pi/project.html")]
    public class ProjectAction : StreamDeckAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectAction"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="displayService">The display service.</param>
        public ProjectAction(ActionInitializationContext context, DisplayService displayService)
            : base(context)
            => this.DisplayService = displayService;

        /// <summary>
        /// Gets the display service.
        /// </summary>
        private DisplayService DisplayService { get; }

        /// <inheritdoc/>
        protected override async Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            if (args.Payload.GetSettings<Settings>()?.Project is ProjectOption value)
            {
                this.DisplayService.SetDisplayConfig(value);
            }
            else
            {
                await this.ShowAlertAsync();
            }
        }

        /// <summary>
        /// Provides settings for the <see cref="ProjectAction"/>.
        /// </summary>
        public record Settings([property: JsonConverter(typeof(ProjectOptionConverter))] ProjectOption? Project);
    }
}
