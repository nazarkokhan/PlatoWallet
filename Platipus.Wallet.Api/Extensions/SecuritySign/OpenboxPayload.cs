namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;
using System.Text;

public static class OpenboxPayload
{
    public static string Encrypt(string data, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] encrypted;

        using (var aesAlg = Aes.Create())
        {
            Array.Clear(aesAlg.IV); // = new byte[16];
            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.Key = keyBytes;

            // AES/ECB/PKCS7Padding
            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(data);
                    }

                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        return Convert.ToBase64String(encrypted);
    }
    
    public static string Decrypt(string data, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] encrypted;

        using (var aesAlg = Aes.Create())
        {
            Array.Clear(aesAlg.IV); // = new byte[16];
            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.Key = keyBytes;

            // AES/ECB/PKCS7Padding
            var encryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(data);
                    }

                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        return Convert.ToBase64String(encrypted);
    }
}