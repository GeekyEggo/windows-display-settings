using Microsoft.Extensions.DependencyInjection;
using MonitorManager.Services;

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
plugin.RunPlugin();
