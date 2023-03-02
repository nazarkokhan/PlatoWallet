namespace Platipus.Wallet.Api.Application.Requests.DTOs;

using Wallets.Psw.Base.Response;

public record GetCurrencyDto(string Id) : PswBaseResponse;