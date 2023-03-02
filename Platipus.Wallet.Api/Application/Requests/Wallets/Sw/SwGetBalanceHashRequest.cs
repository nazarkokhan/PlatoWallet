namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sw;

using Base;
using Base.Response;
using Microsoft.AspNetCore.Mvc;
using Results.ResultToResultMappers;
using Results.Sw;
using Results.Sw.WithData;
using Services.Wallet;

public record SwGetBalanceHashRequest(
    [property: BindProperty(Name = "providerid")] int ProviderId,
    [property: BindProperty(Name = "userid")] int UserId,
    [property: BindProperty(Name = "hash")] string Hash,
    [property: BindProperty(Name = "token")] string Token) : ISwHashRequest, IRequest<ISwResult<SwBalanceResponse>>
{
    public class Handler : IRequestHandler<SwGetBalanceHashRequest, ISwResult<SwBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<ISwResult<SwBalanceResponse>> Handle(
            SwGetBalanceHashRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.GetBalanceAsync(
                request.Token,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToSwResult<SwBalanceResponse>();
            var data = walletResult.Data;

            var response = new SwBalanceResponse(
                data.UserId,
                data.Balance,
                data.Currency);

            return SwResultFactory.Success(response);
        }
    }
}