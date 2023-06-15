using System.Text;

namespace Platipus.Wallet.Api.Extensions.SecuritySign.AtlasPlatform;

public static class AtlasPlatformSecurityBase64
{
    public static bool IsValid(
        string encryptedData, string credentials)
    {
        var decryptedData = Encoding.UTF8.GetString(Convert.FromBase64String(encryptedData));
        return string.Equals(decryptedData, credentials);
    }
}