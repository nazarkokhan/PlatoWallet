using System.Security.Cryptography;
using System.Text;

namespace Platipus.Wallet.Api.Extensions.SecuritySign;

public static class AtlasPlatformSecurityHash
{
    public static bool IsValid(
        string externalHash,
        byte[] data,
        string privateKey)
    {
        var hmacHash = Compute(data, privateKey);

        var isValid = externalHash.Equals(hmacHash, StringComparison.InvariantCultureIgnoreCase);

        return isValid;
    }

    private static string Compute(byte[] data, string privateKey)
    {
        var privateKeyBytes = Encoding.UTF8.GetBytes(privateKey);

        var hmacHashData = HMACSHA512.HashData(privateKeyBytes, data);

        var hmacString = Convert.ToHexString(hmacHashData);

        return hmacString;
    }
}