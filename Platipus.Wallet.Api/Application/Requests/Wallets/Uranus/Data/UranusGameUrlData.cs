namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uranus.Data;

using Newtonsoft.Json;

public sealed record UranusGameUrlData([property: JsonProperty("url")] Uri Url);