namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sw;

using Base;
using Base.Response;
using Microsoft.AspNetCore.Mvc;
using Results.ResultToResultMappers;
using Results.Sw;
using Results.Sw.WithData;
using Services.Wallet;

public record SwGetBalanceMd5Request(
    [property: BindProperty(Name = "providerid")] int ProviderId,
    [property: BindProperty(Name = "userid")] int UserId,
    [property: BindProperty(Name = "md5")] string Md5,
    [property: BindProperty(Name = "token")] string Token) : ISwMd5Request, IRequest<ISwResult<SwBalanceResponse>>
{
    public class Handler : IRequestHandler<SwGetBalanceMd5Request, ISwResult<SwBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<ISwResult<SwBalanceResponse>> Handle(
            SwGetBalanceMd5Request request,
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