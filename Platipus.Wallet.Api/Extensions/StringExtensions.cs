namespace Platipus.Wallet.Api.Extensions;

using System.Text.Json;

public static class StringExtensions
{
    public static string ToCamelCase(this string str)
    {
        return JsonNamingPolicy.CamelCase.ConvertName(str);
    }
}