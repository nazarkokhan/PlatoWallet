namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;
using System.Text;
using Application.Requests.Wallets.Uis.Base;

public static class UisSecurityHash
{
    public static bool IsValid(this IUisHashRequest request, string secretKey)
    {
        var computedHash = request.Compute(secretKey);

        var isValid = request.Hash?.Equals(computedHash, StringComparison.InvariantCultureIgnoreCase);

        return isValid ?? true;
    }

    public static string Compute(this IUisHashRequest request, string secretKey)
    {
        var requestStringToHash = Encoding.UTF8.GetBytes(request.GetSource() + secretKey);

        var md5 = MD5.HashData(requestStringToHash);

        var md5Hex = Convert.ToHexString(md5);

        return md5Hex.ToLower();
    }
}