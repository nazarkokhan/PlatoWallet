namespace Platipus.Wallet.Api.StartupSettings.ControllerSpecificJsonOptions.Formatters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

public class NamedSystemTextJsonInputFormatter : SystemTextJsonInputFormatter
{
    public NamedSystemTextJsonInputFormatter(string settingsName, JsonOptions options, ILogger<NamedSystemTextJsonInputFormatter> logger)
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