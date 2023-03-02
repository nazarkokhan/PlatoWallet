namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;
using System.Text;

public static class UisSecurityHash
{
    public static bool IsValid(string hash, string source, string secretKey)
    {
        var computedHash = Compute(source, secretKey);

        var isValid = hash.Equals(computedHash, StringComparison.InvariantCultureIgnoreCase);

        return isValid;
    }

    public static string Compute(string source, string secretKey)
    {
        var requestStringToHash = Encoding.UTF8.GetBytes(source + secretKey);

        var md5 = MD5.HashData(requestStringToHash);

        var md5Hex = Convert.ToHexString(md5);

        return md5Hex.ToLower();
    }
}