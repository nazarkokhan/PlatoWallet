namespace Platipus.Wallet.Api.Extensions.SecuritySign.Evoplay;

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

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