namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;
using System.Text;

public static class ReevoSecurityHash
{
    public static bool IsValid(
        string key,
        string queryString,
        string secretKey)
    {
        var hash = Compute(queryString, secretKey);

        var isValid = key.Equals(hash, StringComparison.InvariantCultureIgnoreCase);

        return isValid;
    }

    public static string Compute(
        string queryString,
        string secretKey)
    {
        var secretBytes = Encoding.UTF8.GetBytes(secretKey + queryString);
        var hash = SHA1.HashData(secretBytes);

        var hashString = Convert.ToHexString(hash);

        return hashString;
    }
}