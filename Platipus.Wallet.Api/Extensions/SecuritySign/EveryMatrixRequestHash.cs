namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;
using System.Text;

public static class EveryMatrixRequestHash
{
    public static bool IsValidSign(
        string externalMd5,

        string stringToVerify)
    {
        var md5 = Compute(stringToVerify);

        var isValid = externalMd5.Equals(md5);

        return isValid;
    }

    public static string Compute(string stringToVerify)
    {
        var dataBytes = Encoding.UTF8.GetBytes(stringToVerify);

        var md5 = MD5.HashData(dataBytes);
        var md5String = Convert.ToHexString(md5);

        return md5String;
    }
}