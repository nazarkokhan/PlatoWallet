namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay;

using System.ComponentModel;
using Base;
using Data;
using FluentValidation;
using Results.Evoplay;
using Results.Evoplay.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public sealed record EvoplayBalanceRequest(
        [property: DefaultValue("your session token")] string SessionToken, 
        [property: DefaultValue("some player id")] string PlayerId)
    : IEvoplayRequest, IRequest<IEvoplayResult<EvoplaySuccessResponse<EvoplayBalanceData>>>
{
    public sealed class Handler
        : IRequestHandler<EvoplayBalanceRequest, IEvoplayResult<EvoplaySuccessResponse<EvoplayBalanceData>>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) =>
            _walletService = walletService;

        public async Task<IEvoplayResult<EvoplaySuccessResponse<EvoplayBalanceData>>> Handle(
            EvoplayBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.GetBalanceAsync(
                request.SessionToken,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
            {
                walletResult.ToEvoplayFailureResult<EvoplayFailureResponse>();
            }
            
            var data = walletResult.Data;
            var response = new EvoplaySuccessResponse<EvoplayBalanceData>(
                new EvoplayBalanceData(data?.Currency, data!.Balance));
            return EvoplayResultFactory.Success(response);
        }
    }

    public sealed class EvoplayBalanceRequestValidator : AbstractValidator<EvoplayBalanceRequest>
    {
        public EvoplayBalanceRequestValidator()
        {
            RuleFor(x => x.SessionToken)
                .NotEmpty()
                .WithMessage("Session Token is required");
            RuleFor(x => x.PlayerId)
                .NotEmpty()
                .WithMessage("PlayerId is required");
        }
    }
}