namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;
using System.Text;

public static class EmaraplaySecurityHash
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

    public static string Compute(byte[] data, string privateKey)
    {
        var privateKeyBytes = Encoding.UTF8.GetBytes(privateKey);

        var hmacHashData = HMACSHA512.HashData(privateKeyBytes, data);

        var hmacString = Convert.ToHexString(hmacHashData);

        return hmacString;
    }
}