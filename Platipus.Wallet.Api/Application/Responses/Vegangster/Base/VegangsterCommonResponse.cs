namespace Platipus.Wallet.Api.Application.Responses.Vegangster.Base;

public abstract record VegangsterCommonResponse(string Status, string Currency, long Balance);