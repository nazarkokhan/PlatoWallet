namespace Platipus.Wallet.Api.Extensions;

using System.Text;
using System.Text.Json;

public static class StringExtensions
{
    public static string ToCamelCase(this string str)
    {
        return JsonNamingPolicy.CamelCase.ConvertName(str);
    }
    
    public static string ToSnakeCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        var stringBuilder = new StringBuilder();
        stringBuilder.Append(char.ToLowerInvariant(str[0]));

        for (var i = 1; i < str.Length; i++)
        {
            var c = str[i];
            if (char.IsUpper(c))
            {
                stringBuilder.Append('_');
                stringBuilder.Append(char.ToLowerInvariant(c));
            }
            else
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString();
    }
}