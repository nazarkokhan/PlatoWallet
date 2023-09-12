namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Security.Cryptography;
using System.Text;
using Application.Requests.Wallets.Dafabet.Base;

public static class DatabetSecurityHash
{
    public static bool IsValid(this IDafabetRequest request, string requestRoute, string secretKey)
    {
        var computedHash = request.Compute(requestRoute, secretKey);

        var isValid = request.Hash.Equals(computedHash, StringComparison.InvariantCultureIgnoreCase);

        return isValid;
    }

    public static string Compute(string requestRoute, string source, string secretKey)
    {
        var dataToHash = requestRoute + source + secretKey;

        var requestHashBytes = Encoding.UTF8.GetBytes(dataToHash);

        using var crypt = SHA256.Create();

        var computedHashBytes = crypt.ComputeHash(requestHashBytes);

        var computedHash = Convert.ToHexString(computedHashBytes);

        return computedHash.ToLower();
    }

    public static string Compute(this IDafabetRequest request, string requestRoute, string secretKey)
    {
        return Compute(requestRoute, request.GetSource(), secretKey);
    }
}