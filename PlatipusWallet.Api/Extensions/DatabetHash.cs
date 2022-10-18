namespace PlatipusWallet.Api.Extensions;

using System.Security.Cryptography;
using System.Text;
using Application.Requests.Base.Requests;

public static class DatabetHash
{
    public static bool IsValidDatabetHash(this DatabetBaseRequest request, string source, string secretKey)
    {
        var computedHash = Compute(source, secretKey);

        var isValid = request.Hash.Equals(computedHash, StringComparison.InvariantCultureIgnoreCase);

        return isValid;
    }
    
    public static string Compute(string source, string secretKey)
    {
        var requestHashBytes = Encoding.UTF8.GetBytes(source + secretKey);
        
        using var crypt = SHA256.Create();
        
        var computedHashBytes = crypt.ComputeHash(requestHashBytes);
        
        var computedHash = Convert.ToHexString(computedHashBytes);

        return computedHash.ToLower();
    }
}