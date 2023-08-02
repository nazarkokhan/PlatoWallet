namespace Platipus.Wallet.Api.StartupSettings.ControllerSpecificJsonOptions;

using Domain.Entities.Enums;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class JsonSettingsNameAttribute : Attribute
{
    public JsonSettingsNameAttribute(WalletProvider type)
    {
        Name = type.ToString();
        Type = type;
    }

    public string Name { get; }

    public WalletProvider Type { get; }
}