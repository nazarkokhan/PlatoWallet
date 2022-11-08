namespace Platipus.Wallet.Api.StartupSettings.ControllerSpecificJsonOptions.Formatters;

using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Formatters;

public class NamedSystemTextJsonOutputFormatter : SystemTextJsonOutputFormatter
{
    public NamedSystemTextJsonOutputFormatter(string settingsName, JsonSerializerOptions jsonSerializerOptions) : base(jsonSerializerOptions)
    {
        SettingsName = settingsName;
    }

    public string SettingsName { get; }

    public override bool CanWriteResult(OutputFormatterCanWriteContext context)
    {
        if (context.HttpContext.GetJsonSettingsName() != SettingsName)
            return false;
            
        return base.CanWriteResult(context);
    }
}