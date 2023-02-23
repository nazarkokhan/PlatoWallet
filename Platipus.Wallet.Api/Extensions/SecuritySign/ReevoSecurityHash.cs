namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;
using System.Text;

public static class ReevoSecurityHash
{
    public static bool IsValid(
        string key,
        string queryString,
        string secretKey)
    {
        var hash = Compute(queryString, secretKey);

        var isValid = key.Equals(hash, StringComparison.InvariantCultureIgnoreCase);

        return isValid;
    }

    public static string Compute(
        string queryString,
        string secretKey)
    {
        var secretBytes = Encoding.UTF8.GetBytes(queryString + secretKey);
        var hash = SHA1.HashData(secretBytes);

        var hashString = Tools.ToHex(hash);
        var hashString2 = Convert.ToHexString(hash);

        return hashString;
    }
}

public class Tools
{
    static public string ToHex(byte[] data)
    {
        StringBuilder hash = new StringBuilder();
        for (int i = 0; i < 16; ++i)
            hash.AppendFormat("{0:x2}", data[i]);

        return hash.ToString();
    }
}