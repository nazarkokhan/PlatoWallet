namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;
using System.Text;

public static class BetConstructSecurityHash
{
    public static bool IsValid(
        string externalMd5,
        string time,
        string data,
        string secretKey)
    {
        var md5 = Compute(time, data, secretKey);

        var isValid = externalMd5.Equals(md5, StringComparison.InvariantCultureIgnoreCase);

        return isValid;
    }

    public static string Compute(string time, string data, string privateKey)
    {
        var dataString = $"{privateKey}{time}{data}";
        var dataBytes = Encoding.UTF8.GetBytes(dataString);

        var md5 = MD5.HashData(dataBytes);
        var md5String = Convert.ToHexString(md5);

        return md5String;
    }
}