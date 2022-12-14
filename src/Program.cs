[assembly: Manifest(
    Category = "Windows Display Settings",
    CategoryIcon = "Images/Plugin/CategoryIcon",
    Icon = "Images/Plugin/Icon")]

namespace WindowsDisplaySettings
{
    using System.Diagnostics.CodeAnalysis;
    using DisplaySettings.Actions;
    using DisplaySettings.Services;

    /// <summary>
    /// Windows Display Settings.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Project))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SetAdvancedColor))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SetDisplayOrientation))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SetDisplayResolution))]
        private static void Main(string[] args)
        {
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Launch();
            }
#endif

            StreamDeckPlugin.Create()
                .MapPropertyInspectorDataSource("getDisplays", () => DisplayService.GetDisplays())
                .MapPropertyInspectorDataSource("getDisplaysWithHdrSupport", () => DisplayService.GetDisplaysWithHdrSupport())
                .RunPlugin();
        }
    }
}
