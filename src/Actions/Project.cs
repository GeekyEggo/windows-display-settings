namespace DisplaySettings.Actions
{
    using System;
    using System.Threading.Tasks;
    using WindowsDisplayAPI.DisplayConfig;
    using WindowsDisplayAPI.Native.DisplayConfig;

    /// <summary>
    /// Provides an action capable of setting the display configuration, i.e. "Extend", "Clone" etc.
    /// </summary>
    [Action(
        Name = "Project",
        Icon = "Images/Project/Icon",
        StateImage = "Images/Project/Icon",
        PropertyInspectorPath = "pi/project.html",
        SortIndex = 4)]
    public class Project : StreamDeckAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public Project(ActionInitializationContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        protected override async Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            if (args.Payload.GetSettings<Settings>() is { Project: not null } settings
                && settings.Project is DisplayConfigTopologyId.Clone or DisplayConfigTopologyId.Extend or DisplayConfigTopologyId.External or DisplayConfigTopologyId.Internal)
            {
                PathInfo.ApplyTopology(settings.Project.Value, true);
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
        public record Settings(DisplayConfigTopologyId? Project);
    }
}
