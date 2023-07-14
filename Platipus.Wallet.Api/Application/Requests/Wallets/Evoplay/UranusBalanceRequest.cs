namespace Platipus.Wallet.Api.Application.Requests.Wallets.Evoplay;

using System.ComponentModel;
using Base;
using Data;
using FluentValidation;
using Results.ResultToResultMappers;
using Results.Uranus;
using Results.Uranus.WithData;
using Services.Wallet;

public sealed record UranusBalanceRequest(
        [property: DefaultValue("your session token")] string SessionToken, 
        [property: DefaultValue("some player id")] string PlayerId)
    : IUranusRequest, IRequest<IUranusResult<UranusSuccessResponse<UranusBalanceData>>>
{
    public sealed class Handler
        : IRequestHandler<UranusBalanceRequest, IUranusResult<UranusSuccessResponse<UranusBalanceData>>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) =>
            _walletService = walletService;

        public async Task<IUranusResult<UranusSuccessResponse<UranusBalanceData>>> Handle(
            UranusBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.GetBalanceAsync(
                request.SessionToken,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
            {
                walletResult.ToUranusFailureResult<UranusFailureResponse>();
            }
            
            var data = walletResult.Data;
            var response = new UranusSuccessResponse<UranusBalanceData>(
                new UranusBalanceData(data?.Currency, data!.Balance));
            return UranusResultFactory.Success(response);
        }
    }

    public sealed class EvoplayBalanceRequestValidator : AbstractValidator<UranusBalanceRequest>
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