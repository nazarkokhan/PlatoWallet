namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;
using System.Text;

public static class SwRequestMd5
{
    public static bool IsValidSign(
        string externalMd5,
        int providerId,
        int userId,
        string secretKey)
    {
        var md5 = Compute(providerId, userId, secretKey);

        var isValid = externalMd5.Equals(md5);

        return isValid;
    }

    public static string Compute(int providerId, int userId, string secretKey)
    {
        var dataString = $"{providerId.ToString()}{secretKey}{userId.ToString()}";
        var dataBytes = Encoding.UTF8.GetBytes(dataString);

        var md5 = MD5.HashData(dataBytes);
        var md5String = Convert.ToHexString(md5);

        return md5String;
    }
}