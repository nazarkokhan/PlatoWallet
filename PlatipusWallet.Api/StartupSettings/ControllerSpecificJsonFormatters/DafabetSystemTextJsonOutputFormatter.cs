namespace PlatipusWallet.Api.StartupSettings.ControllerSpecificJsonFormatters;

using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Formatters;

public class DafabetSystemTextJsonOutputFormatter : SystemTextJsonOutputFormatter
{
    public DafabetSystemTextJsonOutputFormatter(string settingsName, JsonSerializerOptions jsonSerializerOptions) : base(jsonSerializerOptions)
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