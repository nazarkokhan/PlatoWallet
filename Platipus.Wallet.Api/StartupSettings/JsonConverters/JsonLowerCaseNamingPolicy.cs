namespace Platipus.Wallet.Api.StartupSettings.JsonConverters;

using System.Text.Json;

public class JsonLowerCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return name.ToLower();
    }
}