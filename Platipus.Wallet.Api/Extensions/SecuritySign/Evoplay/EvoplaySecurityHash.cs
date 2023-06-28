namespace Platipus.Wallet.Api.Extensions.SecuritySign.Evoplay;

using System.Security.Cryptography;
using System.Text;

public static class EvoplaySecurityHash
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

        return MD5.HashData(privateKeyBytes, data)
            .ToString();
    }
}