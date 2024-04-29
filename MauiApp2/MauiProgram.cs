using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MauiApp2;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        Console.WriteLine("I got here -> ");
            // Load the appsettings.json file
            var executingAssembly = Assembly.GetExecutingAssembly();
            var embeddedAssetNames = executingAssembly.GetManifestResourceNames();
#if DEBUG
            Console.Error.WriteLine($"Found Embedded Assets: {string.Join("\n", embeddedAssetNames)}");
#endif
            var configurationBuilder = new ConfigurationBuilder();
            foreach (var fileName in embeddedAssetNames.Where(x => Regex.IsMatch(x, @"appsettings(.+)?\.json$")))
            {
                using var appsettings = executingAssembly.GetManifestResourceStream(fileName);
                if (appsettings == null)
                    throw new ArgumentNullException(nameof(appsettings), "appsettings.json file was not found.");
                configurationBuilder.AddConfiguration(new ConfigurationBuilder().AddJsonStream(appsettings).Build());
            }

            // Build the config using found json files
            var config = configurationBuilder.Build();
            // Create the MAUI builder
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); })
                .ConfigureEssentials(essentials => { essentials.UseVersionTracking(); });

            // Setup logging
            builder.Services.AddLogging(options =>
            {
                // Set minimum logging to debug when running locally
                options.SetMinimumLevel(System.Diagnostics.Debugger.IsAttached ? LogLevel.Debug : LogLevel.Information);
#if DEBUG
                options.AddDebug();

#endif
            });
            // Bind the configuration
            // Enable Blazor Web View
            builder.Services.AddMauiBlazorWebView();
            // Grab the Base URL
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif
#if IOS
#endif
            return builder.Build();
    }
}