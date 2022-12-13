using DisplaySettings.Services;

[assembly: Manifest(
    Category = "Windows Display Settings",
    CategoryIcon = "Images/Plugin/CategoryIcon",
    Icon = "Images/Plugin/Icon")]

#if DEBUG
if (!System.Diagnostics.Debugger.IsAttached)
{
    System.Diagnostics.Debugger.Launch();
}
#endif

StreamDeckPlugin.Create()
    .MapPropertyInspectorDataSource("getDisplays", () => DisplayService.GetDisplays())
    .MapPropertyInspectorDataSource("getDisplaysWithHdrSupport", () => DisplayService.GetDisplaysWithHdrSupport())
    .MapPropertyInspectorDataSource("getResolutions", () => DisplayService.GetResolutions())
    .RunPlugin();
