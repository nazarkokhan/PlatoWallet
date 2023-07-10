namespace Platipus.Wallet.Api.Application.Requests.Wallets.Atlas;

using Base;
using FluentValidation;
using Responses.AtlasPlatform;
using Platipus.Wallet.Api.Application.Services.Wallet;
using Results.Atlas.WithData;
using Results.ResultToResultMappers;

public sealed record AtlasWinRequest(
    string Token, 
    string ClientId,
    string RoundId, 
    decimal Amount,
    string TransactionId) : 
        IRequest<IAtlasResult<AtlasCommonResponse>>, IAtlasRequest
{
    public sealed class Handler : 
        IRequestHandler<AtlasWinRequest, IAtlasResult<AtlasCommonResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => 
            _walletService = walletService;

        public async Task<IAtlasResult<AtlasCommonResponse>> Handle(
            AtlasWinRequest request, CancellationToken cancellationToken)
        {
            var validAmount = request.Amount / 100;
            var walletResult = await _walletService.WinAsync(
                request.Token,
                request.RoundId,
                request.TransactionId,
                validAmount, 
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToAtlasFailureResult<AtlasCommonResponse>();

            var data = walletResult.Data;
            var response = new AtlasCommonResponse(
                data?.Currency!, 
                (long)(data!.Balance * 100), 
                data.UserId.ToString()
            );
            return walletResult.ToAtlasResult(response);
        }
    }
    
    public sealed class AtlasWinRequestValidator : AbstractValidator<AtlasWinRequest>
    {
        public AtlasWinRequestValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("Token is required.");

            RuleFor(x => x.ClientId)
                .NotEmpty()
                .WithMessage("ClientId is required.");

            RuleFor(x => x.RoundId)
                .NotEmpty()
                .WithMessage("RoundId is required.")
                .MaximumLength(150)
                .WithMessage("RoundId cannot exceed 150 characters.");

            RuleFor(x => x.Amount)
                .NotEmpty()
                .WithMessage("Amount property is required");

            RuleFor(x => x.TransactionId)
                .NotEmpty()
                .WithMessage("TransactionId is required.")
                .MaximumLength(150)
                .WithMessage("TransactionId cannot exceed 150 characters.");
        }
    }

}