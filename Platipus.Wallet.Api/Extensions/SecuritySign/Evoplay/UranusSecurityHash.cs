namespace Platipus.Wallet.Api.Extensions.SecuritySign.Evoplay;

using System.Security.Cryptography;
using System.Text;

public static class UranusSecurityHash
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
        using var md5 = MD5.Create();
        var input = $"{requestJson}{privateKey}";
        var data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

        return string.Concat(data.Select(x => x.ToString("x2")));
    }
}