namespace Platipus.Wallet.Api.Extensions.SecuritySign.Atlas;

using System.Security.Cryptography;
using System.Text;

public static class AtlasSecurityHash
{
    public static bool IsValid(
        string externalHash,
        byte[] data,
        string privateKey)
    {
        var md5Hash = Compute(data, privateKey);

        var isValid = externalHash.Equals(md5Hash, StringComparison.InvariantCultureIgnoreCase);

        return isValid;
    }

    private static string Compute(byte[] data, string privateKey)
    {
        // Convert the private key to bytes
        var keyBytes = Encoding.UTF8.GetBytes(privateKey);
        using var hmacMd5 = new HMACMD5(keyBytes);

        // Compute the HMAC-MD5 hash of the data
        var hashData = hmacMd5.ComputeHash(data);

        // Convert the hash data to a hexadecimal string
        var hashString = BitConverter.ToString(hashData)
            .Replace("-", "")
            .ToLowerInvariant();

        return hashString;
    }
}