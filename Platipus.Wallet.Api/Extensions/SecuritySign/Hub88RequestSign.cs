namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;

public static class Hub88RequestSign
{
    public static bool IsValidSign(string requestSign, byte[] rawRequestBody, string signatureKey)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(signatureKey);

        var isValid = rsa.VerifyData(
            rawRequestBody,
            Convert.FromBase64String(requestSign),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        return isValid;
    }

    public static string Compute(byte[] rawRequestBody, string signatureKey)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(signatureKey);

        var customEncrypted = rsa.SignData(rawRequestBody, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        var validSignature = Convert.ToBase64String(customEncrypted);

        return validSignature;
    }
}