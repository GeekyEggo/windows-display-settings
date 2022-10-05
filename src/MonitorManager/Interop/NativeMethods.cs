namespace MonitorManager.Interop
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Collection of native methods.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// The apply value, used in union when setting the <see cref="SetDisplayConfig(uint, IntPtr, uint, IntPtr, uint)"/>.
        /// </summary>
        public static readonly uint SDC_APPLY = 0x00000080;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern long SetDisplayConfig(uint numPathArrayElements, IntPtr pathArray, uint numModeArrayElements, IntPtr modeArray, uint flags);
    }
}
