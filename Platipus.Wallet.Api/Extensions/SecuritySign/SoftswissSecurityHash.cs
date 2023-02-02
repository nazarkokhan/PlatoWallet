namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;
using System.Text;

public static class SoftswissSecurityHash
{
    public static bool IsValid(string requestSign, byte[] rawRequestBody, string signatureKey)
    {
        var computedHash = Compute(rawRequestBody, signatureKey);

        var isValid = requestSign.Equals(computedHash, StringComparison.InvariantCultureIgnoreCase);

        return isValid;
    }

    public static string Compute(byte[] rawRequestBody, string signatureKey)
    {
        var signatureKeyBytes = Encoding.UTF8.GetBytes(signatureKey);
        var hmac = HMACSHA256.HashData(signatureKeyBytes, rawRequestBody);
        var validSignature = Convert.ToHexString(hmac);

        return validSignature.ToLower();
    }
}