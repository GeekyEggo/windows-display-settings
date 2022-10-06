namespace DisplaySettings.Actions
{
    using System;

    [Action(
        Name = "Scale",
        Icon = "Images/SetScale/Icon",
        StateImage = "Images/SetScale/Icon")]
    public class SetScale : StreamDeckAction
    {
        public SetScale(ActionInitializationContext context)
            : base(context)
        {
        }
    }
}
