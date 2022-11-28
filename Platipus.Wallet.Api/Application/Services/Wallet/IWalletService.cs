namespace Platipus.Wallet.Api.Application.Services.Wallet;

using DTOs;

public interface IWalletService
{
    Task<IResult<BalanceResponse>> GetBalanceAsync(
        GetBalanceRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<BetOrWinResponse>> BetAsync(
        BetRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<BetOrWinResponse>> WinAsync(
        WinRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<BetOrWinResponse>> RollbackAsync(
        RollbackRequest request,
        CancellationToken cancellationToken = default);

    Task<IResult<BalanceResponse>> AwardAsync(
        AwardRequest request,
        CancellationToken cancellationToken = default);
}