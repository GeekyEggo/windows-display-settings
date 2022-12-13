using DisplaySettings.Services;

[assembly: Manifest(
    Category = "Display Settings",
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
    .MapPropertyInspectorDataSource("getResolutions", () => DisplayService.GetResolutions())
    .RunPlugin();
