namespace DisplaySettings.Actions
{
    using System;
    using System.Threading.Tasks;
    using DisplaySettings.Extensions;
    using DisplaySettings.Services;
    using StreamDeck.Events;
    using WindowsDisplayAPI;

    [Action(
        Name = "HDR",
        Icon = "Images/SetDisplayOrientation/Icon",
        StateImage = "Images/SetDisplayOrientation/Icon",
        PropertyInspectorPath = "pi/set-hdr.html")]
    public class SetHdr : StreamDeckAction
    {
        /*
         * Todo:
         * - Create an icon.
         * - Document.
         * - Remove the display service.
         */
        public SetHdr(ActionInitializationContext context, DisplayService displayService)
            : base(context) => this.DisplayService = displayService;

        public DisplayService DisplayService { get; }

        protected override async Task OnKeyDown(ActionEventArgs<KeyPayload> args)
        {
            if (args.Payload.GetSettings<Settings>() is { DeviceName: not null } settings
                && Display.GetDisplays().FirstOrDefault(d => d.DisplayName == settings.DeviceName) is Display display and not null
                && display.GetAdvancedColorInfo() is DisplayAdvancedColorInfo colorInfo and { AdvancedColorSupported: true })
            {
                if (!colorInfo.AdvancedColorEnabled && settings.Action is HdrAction.Toggle or HdrAction.On)
                {
                    display.SetAdvancedColorState(true);
                    await this.ShowOkAsync();
                }
                else if (colorInfo.AdvancedColorEnabled && settings.Action is HdrAction.Toggle or HdrAction.Off)
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
        public record Settings(string? DeviceName, HdrAction Action = HdrAction.Toggle);

        public enum HdrAction
        {
            Toggle = 0,
            On = 1,
            Off = 2
        }
    }
}
