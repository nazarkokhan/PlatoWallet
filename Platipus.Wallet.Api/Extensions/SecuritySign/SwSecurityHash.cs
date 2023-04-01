namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;
using System.Text;

public static class SwSecurityHash
{
    public static bool IsValid(
        string externalHash,
        int providerId,
        int userId,
        string secretKey)
    {
        var hash = Compute(providerId, userId, secretKey);

        var isValid = externalHash.Equals(hash, StringComparison.InvariantCultureIgnoreCase);

        return isValid;
    }

    public static string Compute(int providerId, int userId, string secretKey)
    {
        var dataString = $"{providerId.ToString()}{secretKey}{userId.ToString()}";
        var dataBytes = Encoding.UTF8.GetBytes(dataString);

        var hash = SHA256.HashData(dataBytes);
        var hashString = Convert.ToHexString(hash);

        return hashString;
    }
}