namespace MonitorManager.Services
{
    using MonitorManager.Interop;

    /// <summary>
    /// Provides methods and interaction for displays.
    /// </summary>
    public class DisplayService
    {
        /// <summary>
        /// Sets the display configuration, i.e. "Clone", "Extend", etc..
        /// </summary>
        /// <param name="value">The desired projection.</param>
        public void SetDisplayConfig(ProjectOption value)
        {
            if (value is not ProjectOption.Clone
                and not ProjectOption.Extend
                and not ProjectOption.External
                and not ProjectOption.Internal)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "The prefered projection is not a valid option.");
            }

            NativeMethods.SetDisplayConfig(0, IntPtr.Zero, 0, IntPtr.Zero, (NativeMethods.SDC_APPLY | (uint)value));
        }
    }
}
