namespace Platipus.Wallet.Api.Extensions.SecuritySign;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class BetConstructVerifyHashExtension
{
    public static bool VerifyBetConstructHash(object request, string hash, DateTime time)
    {
        var jObject = JsonConvert.SerializeObject(request);

        var data = JObject.Parse(jObject);

        data
            .Properties()
            .First(attr => attr.Name == "Hash")
            .Remove();

        data.Properties()
            .First(attr => attr.Name == "Time")
            .Remove();

        data.Properties()
            .First(attr => attr.Name == "Data")
            .Remove();

        var jStringData = data.ToString();

        var isValidHash = BetConstructRequestHash.IsValidSign(hash, time.ToString(CultureInfo.InvariantCulture), jStringData);

        return isValidHash;
    }
}