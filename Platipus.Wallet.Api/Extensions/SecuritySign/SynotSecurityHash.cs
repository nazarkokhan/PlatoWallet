namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

public static class SynotSecurityHash
{
    public static bool IsValid(
        string externalHash,
        string? bodyJson,
        string privateKey,
        string xEasToken)
    {
        var hmacHash = Compute(bodyJson, privateKey, xEasToken);

        var isValid = externalHash.Equals(hmacHash, StringComparison.InvariantCultureIgnoreCase);

        return isValid;
    }

    private static string Compute(string? input, string privateKey, string xEasToken)
    {
        var inputString = ExtractStringFromInput(input);
        string finalInput;
        if (string.IsNullOrEmpty(inputString) && !string.IsNullOrWhiteSpace(xEasToken))
        {
            finalInput = xEasToken;
        }
        else
        {
            finalInput = string.IsNullOrEmpty(xEasToken)
                ? inputString
                : $"{inputString}|{xEasToken}";
        }

        var inputBytes = Encoding.UTF8.GetBytes(finalInput);
        var secretKeyBytes = Encoding.UTF8.GetBytes(privateKey);
        using var hmac = new HMACSHA256(secretKeyBytes);

        var resultHmacHashData = hmac.ComputeHash(inputBytes);

        // Convert byte array to a lower-case hexadecimal string
        var sb = new StringBuilder(resultHmacHashData.Length * 2);
        foreach (var b in resultHmacHashData)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
    
    private static string ExtractStringFromInput(object? input)
    {
        switch (input)
        {
            case Dictionary<string, string> dictInput:
                return string.Join("|", dictInput.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value));

            case string jsonString:
            {
                // Parse the input string as JSON
                var jsonObject = JObject.Parse(jsonString);
                return ExtractStringFromJson(jsonObject);
            }

            default:
                return string.Empty; // Fallback if none of the above cases match
        }
    }
    
    private static string ExtractStringFromJson(JToken token)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                return string.Join(
                    "|",
                    ((JObject)token).Properties().OrderBy(p => p.Name).Select(p => ExtractStringFromJson(p.Value)));

            case JTokenType.Array:
                return string.Join("|", ((JArray)token).Select(ExtractStringFromJson));

            case JTokenType.String:
            case JTokenType.Integer:
            case JTokenType.Float:
            case JTokenType.Date:
            case JTokenType.Guid:
            case JTokenType.TimeSpan:
                return ((JValue)token).Value?.ToString()!;

            case JTokenType.Boolean:
                return ((JValue)token).Value?.ToString()?.ToLower()!;

            default:
                return string.Empty;
        }
    }
}