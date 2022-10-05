namespace MonitorManager.Actions
{
    [Action(
        Name = "Orientation",
        Icon = "Images/SetDisplayOrientation/Icon",
        StateImage = "Images/SetDisplayOrientation/Icon")]
    public class SetDisplayOrientation : StreamDeckAction
    {
        // Ref: https://gist.github.com/umq/986635
        public SetDisplayOrientation(ActionInitializationContext context)
            : base(context)
        {
        }
    }
}
