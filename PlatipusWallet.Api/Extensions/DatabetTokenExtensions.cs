namespace PlatipusWallet.Api.Extensions;

using System.Security.Cryptography;
using System.Text;
using Application.Requests.Base.Requests;

public static class DatabetTokenExtensions
{
    public static bool IsValidDatabetHash(this DatabetBaseRequest request, string source, string secretKey)
    {
        var requestHashBytes = Encoding.UTF8.GetBytes(source + secretKey);
        
        using var crypt = SHA256.Create();
        
        var computedHashBytes = crypt.ComputeHash(requestHashBytes);
        
        var computedHash = Convert.ToHexString(computedHashBytes);

        var isValid = request.Hash.Equals(computedHash, StringComparison.InvariantCultureIgnoreCase);

        return isValid;
    }
}