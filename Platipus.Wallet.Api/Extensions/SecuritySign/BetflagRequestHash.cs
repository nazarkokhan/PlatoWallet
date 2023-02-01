namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;
using System.Text;

public static class BetflagRequestHash
{
    public static bool IsValidSign(
        string externalMd5,
        string firstValue,
        long timestamp,
        string secretKey)
    {
        var md5 = Compute(firstValue, timestamp, secretKey);

        var isValid = externalMd5.Equals(md5, StringComparison.InvariantCultureIgnoreCase);

        return isValid;
    }

    public static string Compute(string firstValue, long timestamp, string secretKey)
    {
        var dataString = $"{firstValue}{timestamp.ToString()}{secretKey}";
        var dataBytes = Encoding.UTF8.GetBytes(dataString);

        var md5 = MD5.HashData(dataBytes);
        var md5String = Convert.ToHexString(md5);

        return md5String;
    }
}