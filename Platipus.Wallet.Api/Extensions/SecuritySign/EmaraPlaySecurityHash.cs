using System.Security.Cryptography;
using System.Text;

namespace Platipus.Wallet.Api.Extensions.SecuritySign;

public static class EmaraPlaySecurityHash
{
    public static bool IsValid(
        string externalHash,
        byte[] requestBodyBytes,
        string privateKey)
    {
        var correctHash = Compute(requestBodyBytes, privateKey);

        var isValid = externalHash.Equals(correctHash, StringComparison.InvariantCultureIgnoreCase);

        return isValid;
    }

    public static string Compute(byte[] data, string privateKey)
    {
        var privateKeyBytes = Encoding.UTF8.GetBytes(privateKey);

        var resultHmacHashData = HMACSHA512.HashData(privateKeyBytes, data);

        var result = Convert.ToHexString(resultHmacHashData);

        return result;
    }

}