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
        var stringToVerify = $"{nameOfMethod}{DateTime.UtcNow:yyyy:MM:dd:hh}{password}";

        var dataBytes = Encoding.ASCII.GetBytes(stringToVerify);

        var md5 = MD5.HashData(dataBytes);
        var md5String = Convert.ToHexString(md5);

        return md5String.ToLower();
    }

    // static public string GetMD5(string str, int size)
    // {
    //     if (size > 0 && str.Length != size) str.PadLeft(16, '#');
    //
    //     MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
    //     return ToHex(md5.ComputeHash(Encoding.ASCII.GetBytes(str)));
    // }
}