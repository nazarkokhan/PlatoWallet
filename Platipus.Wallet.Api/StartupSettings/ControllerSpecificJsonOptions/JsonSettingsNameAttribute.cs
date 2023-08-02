namespace Platipus.Wallet.Api.StartupSettings.ControllerSpecificJsonOptions;

using Domain.Entities.Enums;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class JsonSettingsNameAttribute : Attribute
{
    public JsonSettingsNameAttribute(CasinoProvider type)
    {
        Name = type.ToString();
        Type = type;
    }

    public string Name { get; }

    public CasinoProvider Type { get; }
}