namespace MonitorManager.Interop
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Collection of native methods.
    /// </summary>
    internal static class User32
    {
        /// <summary>
        /// The apply value, used in union when setting the <see cref="SetDisplayConfig(uint, IntPtr, uint, IntPtr, uint)"/>.
        /// </summary>
        public const uint SDC_APPLY = 0x00000080;

        /// <summary>
        /// Used by <see cref="EnumDisplaySettings(string, int, ref DEVMODE)"/> to retrieve the current settings.
        /// </summary>
        public const int ENUM_CURRENT_SETTINGS = -1;

        /// <summary>
        /// The result of <see cref="ChangeDisplaySettings(ref DEVMODE, int)"/> was successful.
        /// </summary>
        public const int DISP_CHANGE_SUCCESSFUL = 0;

        /// <summary>
        /// The result of <see cref="ChangeDisplaySettings(ref DEVMODE, int)"/> requires a restart.
        /// </summary>
        public const int DISP_CHANGE_RESTART = 1;

        /// <summary>
        /// The result of <see cref="ChangeDisplaySettings(ref DEVMODE, int)"/> failed.
        /// </summary>
        public const int DISP_CHANGE_FAILED = -1;

        [DllImport("user32.dll")]
        public static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern long SetDisplayConfig(uint numPathArrayElements, IntPtr pathArray, uint numModeArrayElements, IntPtr modeArray, uint flags);
    }
}
