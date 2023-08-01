namespace Platipus.Wallet.Api.StartupSettings.ControllerSpecificJsonOptions;

using Domain.Entities.Enums;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class JsonSettingsNameAttribute : Attribute
{
    public JsonSettingsNameAttribute(CasinoProvider type)
        : this(type.ToString())
    {
    }

    //TODO remove. Use new constructor
    public JsonSettingsNameAttribute(string name)
    {
        Name = name;
        Type = Enum.Parse<CasinoProvider>(name);
    }

    public string Name { get; }

    public CasinoProvider Type { get; }
}