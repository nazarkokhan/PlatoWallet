namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;
using System.Text;

public static class SwSecurityMd5
{
    public static bool IsValidSign(
        string externalMd5,
        int providerId,
        int userId,
        string secretKey,
        string? amount = null)
    {
        var md5 = Compute(
            providerId,
            userId,
            secretKey,
            amount);

        var isValid = externalMd5.Equals(md5, StringComparison.InvariantCultureIgnoreCase);

        return isValid;
    }

    public static string Compute(
        int providerId,
        int userId,
        string secretKey,
        string? amount = null)
    {
        var dataString = $"{providerId.ToString()}{secretKey}{userId.ToString()}{amount}";
        var dataBytes = Encoding.UTF8.GetBytes(dataString);

        var md5 = MD5.HashData(dataBytes);
        var md5String = Convert.ToHexString(md5);

        return md5String;
    }
}