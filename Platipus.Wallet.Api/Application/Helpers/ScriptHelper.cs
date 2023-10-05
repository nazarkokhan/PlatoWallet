namespace Platipus.Wallet.Api.Application.Helpers;

using System.Text;
using System.Text.RegularExpressions;

public ref struct ScriptHelper
{
    public static string ExtractUrlFromScript(string jsScript, string environment)
    {
        return environment switch
        {
            "local" => ParseLocal(jsScript),
            "wbg" or "gameserver-test" or "gs-everymatrix" => ParseWbg(jsScript),
            "test" => ParseTest(jsScript),
            _ => string.Empty
        };
    }

    private static string ParseLocal(string jsScript)
    {
        const string startDelimiter = "window.location.assign(\"";
        const string endDelimiter = ");";
        const string unwantedSuffix = "\" + lobbyURL";

        var startPos = jsScript.IndexOf(startDelimiter, StringComparison.Ordinal);
        var endPos = jsScript.LastIndexOf(endDelimiter, StringComparison.Ordinal);

        if (startPos == -1 || endPos == -1)
            return string.Empty;

        var extractedUrl = jsScript.Substring(startPos + startDelimiter.Length, endPos - startPos - startDelimiter.Length);
        var suffixPos = extractedUrl.IndexOf(unwantedSuffix, StringComparison.Ordinal);

        return suffixPos == -1 ? extractedUrl : extractedUrl[..suffixPos];
    }

    private static string ParseWbg(string jsScript)
    {
        var lobbyUrlValue = ExtractValue(jsScript, @"const lobbyURL = encodeURIComponent\('([^']*)'\);");
        var langValue = ExtractValue(jsScript, @"const lang = '([^']*)'.toLowerCase\(\);")?.ToLower();

        var langCodeValue = langValue switch
        {
            "zh" => "zh-cn",
            "zt" => "zh-tw",
            _ => langValue
        };

        return BuildUrlFromTemplate(
            jsScript,
            @"window.location.assign\(([^\)]+)\);",
            lobbyUrlValue,
            langCodeValue);
    }

    private static string ParseTest(string jsScript)
    {
        var lobbyUrlValue = ExtractValue(jsScript, @"var lobbyURL = encodeURIComponent\('([^']*)'\);");
        return BuildUrlFromTemplate(jsScript, "const gameURL = ([^;]+);", lobbyUrlValue);
    }

    private static string? ExtractValue(string input, string pattern)
    {
        var match = Regex.Match(input, pattern);
        return match.Success ? match.Groups[1].Value : null;
    }

    private static string BuildUrlFromTemplate(
        string jsScript,
        string urlPattern,
        string? lobbyUrlValue,
        string? langCodeValue = null)
    {
        var urlMatch = Regex.Match(jsScript, urlPattern);
        if (!urlMatch.Success)
            return string.Empty;

        var urlTemplate = urlMatch.Groups[1].Value;
        var parts = urlTemplate.Split('+');
        var finalUrl = new StringBuilder();

        foreach (var part in parts)
        {
            var trimmedPart = part.Trim();
            if (trimmedPart.StartsWith('"') && trimmedPart.EndsWith('"'))
            {
                finalUrl.Append(trimmedPart.Trim('"'));
            }
            else
            {
                switch (trimmedPart)
                {
                    case "lobbyURL":
                        if (lobbyUrlValue is not null)
                            finalUrl.Append(Uri.EscapeDataString(lobbyUrlValue));

                        break;

                    case "langCode":
                        finalUrl.Append(langCodeValue);
                        break;

                    default:
                        finalUrl.Append(trimmedPart);
                        break;
                }
            }
        }

        return finalUrl.ToString();
    }
}