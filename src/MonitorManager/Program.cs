using Microsoft.Extensions.DependencyInjection;
using MonitorManager.Services;
using StreamDeck.Extensions.PropertyInspectors;

[assembly: Manifest(Category = "Monitor Manager")]

#if DEBUG
if (!System.Diagnostics.Debugger.IsAttached)
{
    System.Diagnostics.Debugger.Launch();
}
#endif

var builder = StreamDeckPlugin.CreateBuilder();
builder.ConfigureServices(s => s.AddSingleton<DisplayService>());

var plugin = builder.Build();
plugin.MapPropertyInspectorDataSource("getDisplays", (DisplayService displayService) => displayService.GetDisplays());
plugin.MapPropertyInspectorDataSource("getResolutions", (DisplayService displayService) => displayService.GetResolutions());

plugin.RunPlugin();
