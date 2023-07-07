namespace Platipus.Wallet.Api.Application.Requests.Wallets.Atlas;

using Base;
using FluentValidation;
using Responses.AtlasPlatform;
using Platipus.Wallet.Api.Application.Services.Wallet;
using Results.Atlas;
using Results.Atlas.WithData;
using Results.ResultToResultMappers;

public sealed record AtlasGetClientBalanceRequest(
    string Token) : 
    IAtlasRequest, IRequest<IAtlasResult<AtlasCommonResponse>>
{
    public sealed class Handler :
        IRequestHandler<AtlasGetClientBalanceRequest,
            IAtlasResult<AtlasCommonResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => 
            _walletService = walletService;

        public async Task<IAtlasResult<AtlasCommonResponse>> Handle(
            AtlasGetClientBalanceRequest request, CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.GetBalanceAsync(
                request.Token,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
            {
                walletResult.ToAtlasFailureResult<AtlasCommonResponse>();
            }

            var data = walletResult.Data;
            var response = new AtlasCommonResponse(
                data?.Currency!, 
                (long)(walletResult.Data!.Balance * 100), 
                data?.UserId.ToString()!
                );
            return walletResult.ToAtlasResult(response);
        }
    }

    public sealed class AtlasGetClientBalanceRequestValidator : AbstractValidator<AtlasGetClientBalanceRequest>
    {
        public AtlasGetClientBalanceRequestValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("Token is required");
        }
    }
}