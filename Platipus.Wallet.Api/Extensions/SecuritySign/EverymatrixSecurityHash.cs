namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;
using System.Text;

public static class EverymatrixSecurityHash
{
    public static bool IsValidSign(
        string requestSign,
        string nameOfMethod,
        string password)
    {
        var hash = Compute(nameOfMethod, password);

        var isValid = requestSign.Equals(hash, StringComparison.InvariantCultureIgnoreCase);

        return isValid;
    }

    public static string Compute(string nameOfMethod, string password)
    {
        var stringToVerify = $"{nameOfMethod}{DateTime.Now:yyyy:MM:dd:hh}{password}";

        var dataBytes = Encoding.ASCII.GetBytes(stringToVerify);

        var md5 = MD5.HashData(dataBytes);
        var md5String = Convert.ToHexString(md5);

        return md5String.ToLower();
    }
}