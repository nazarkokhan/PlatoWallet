namespace PlatipusWallet.Api.StartupSettings.ControllerSpecificJsonFormatters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

public class DafabetSystemTextJsonInputFormatter : SystemTextJsonInputFormatter
{
    public DafabetSystemTextJsonInputFormatter(string settingsName, JsonOptions options, ILogger<DafabetSystemTextJsonInputFormatter> logger)
        : base(options, logger)
    {
        SettingsName = settingsName;
    }

    public string SettingsName { get; }

    public override bool CanRead(InputFormatterContext context)
    {
        if (context.HttpContext.GetJsonSettingsName() != SettingsName)
            return false;

        return base.CanRead(context);
    }
}