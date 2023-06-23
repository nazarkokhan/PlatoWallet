namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;

using System.Text.Json.Serialization;
using Base;
using StartupSettings.JsonConverters;

public sealed record RoundDetailsResult(
    string Type, 
    string Details, 
    [property: JsonConverter(typeof(JsonNumberStringAsBoolConverter))]bool Completed) : IEmaraPlayBaseResponse;