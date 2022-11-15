namespace Platipus.Wallet.Api.Application.Services.Wallet;

using DTOs;

public interface IWalletService
{
    Task<IResult<BalanceResponse>> GetBalanceAsync(
        GetBalanceRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<BalanceResponse>> BetAsync(
        BetRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<BalanceResponse>> WinAsync(
        WinRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<BalanceResponse>> RollbackAsync(
        RollbackRequest request,
        CancellationToken cancellationToken = default);
}