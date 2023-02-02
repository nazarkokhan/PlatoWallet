namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;
using System.Text;

public static class OpenboxSecurityPayload
{
    public static string Encrypt(string data, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] encrypted;

        using (var aesAlg = Aes.Create())
        {
            Array.Clear(aesAlg.IV);
            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.Key = keyBytes;

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(
                           msEncrypt,
                           encryptor,
                           CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(data);
                    }

                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        var base64 = Convert.ToBase64String(encrypted);

        return base64;
    }

    public static string Decrypt(string data, string key)
    {
        var cipherText = Convert.FromBase64String(data);
        var keyBytes = Encoding.UTF8.GetBytes(key);

        using var aesAlg = Aes.Create();
        Array.Clear(aesAlg.IV);
        aesAlg.Mode = CipherMode.ECB;
        aesAlg.Padding = PaddingMode.PKCS7;
        aesAlg.Key = keyBytes;

        var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using var msDecrypt = new MemoryStream(cipherText);
        using var csDecrypt = new CryptoStream(
            msDecrypt,
            decryptor,
            CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        var plaintext = srDecrypt.ReadToEnd();

        return plaintext;
    }
}