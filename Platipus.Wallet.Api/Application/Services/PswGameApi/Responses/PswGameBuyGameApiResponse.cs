namespace Platipus.Wallet.Api.Application.Services.PswGameApi.Responses;

using Platipus.Wallet.Api.Application.Requests.Wallets.Psw.Base.Response;

public record PswGameBuyGameApiResponse(long RoundId) : PswBaseResponse;