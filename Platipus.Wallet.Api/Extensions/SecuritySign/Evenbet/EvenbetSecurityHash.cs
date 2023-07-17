namespace Platipus.Wallet.Api.Extensions.SecuritySign.Evenbet;

using System.Security.Cryptography;
using System.Text;

public static class EvenbetSecurityHash
{
    public static bool IsValid(
        string externalHash,
        string requestJson,
        string privateKey)
    {
        var hmacHash = Compute(requestJson, privateKey);

        var isValid = externalHash.Equals(hmacHash, StringComparison.InvariantCultureIgnoreCase);

        return isValid;
    }

    private static string Compute(string requestJson, string privateKey)
    {
        var toHash = $"{requestJson}{privateKey}";

        using var hash = SHA256.Create();
        var result = hash.ComputeHash(Encoding.UTF8.GetBytes(toHash));

        return string.Concat(result.Select(x => x.ToString("x2")));
    }
}