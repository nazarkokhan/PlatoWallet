namespace Platipus.Wallet.Api.Application.Requests.External.Sw;

using System.Diagnostics.CodeAnalysis;

public record SwKeyData(string CasinoId, string Currency)
{
    public static bool TryParse(string key, [MaybeNullWhen(false)] out SwKeyData keyData)
    {
        keyData = null;

        if (string.IsNullOrEmpty(key) || key.Length < 3)
            return false;

        var lastIndex = key.LastIndexOf('-');
        if (lastIndex <= 0 || lastIndex >= key.Length - 1)
            return false;

        var casinoId = key[..lastIndex];
        var currency = key[(lastIndex + 1)..];

        keyData = new SwKeyData(casinoId, currency);
        return true;
    }
};