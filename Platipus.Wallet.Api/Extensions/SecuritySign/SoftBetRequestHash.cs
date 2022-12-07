namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;
using System.Text;

public static class SoftBetRequestHash
{
    public static bool IsValidSign(
        string externalHash,
        byte[] data,
        string secretKey)
    {
        var hash = Compute(data, secretKey);

        var isValid = externalHash.Equals(hash, StringComparison.InvariantCultureIgnoreCase);

        return isValid;
    }

    public static string Compute(byte[] data, string secretKey)
    {
        var secretBytes = Encoding.UTF8.GetBytes(secretKey);

        var hash = HMACSHA256.HashData(secretBytes, data);
        var hashString = Convert.ToHexString(hash);

        return hashString;
    }
}