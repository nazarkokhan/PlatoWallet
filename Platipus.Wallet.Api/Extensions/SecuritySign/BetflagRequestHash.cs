namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;
using System.Text;

public static class BetflagRequestHash
{

    public static bool IsValidSign(
        string externalMd5,
        string sessionId,
        long timeStamp,
        string secretKey = "BetflagSecretKey")
    {
        var md5 = Compute(sessionId, timeStamp, secretKey).ToUpperInvariant();

        var isValid = externalMd5.Equals(md5);

        return isValid;
    }

    public static string Compute(string sessionId, long timeStamp, string secretKey = "BetflagSecretKey")
    {
        var dataString = $"{sessionId}{timeStamp.ToString()}{secretKey}";
        var dataBytes = Encoding.UTF8.GetBytes(dataString);

        var md5 = MD5.HashData(dataBytes);
        var md5String = Convert.ToHexString(md5);

        return md5String;
    }
}