namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sw;

using Base;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Results.ResultToResultMappers;
using Results.Sw;
using Results.Sw.WithData;
using Services.Wallet;

public record SwGetUserNameRequest(
        [property: BindProperty(Name = "providerid")] int ProviderId,
        [property: BindProperty(Name = "userid")] int UserId,
        [property: BindProperty(Name = "hash")] string Hash,
        [property: BindProperty(Name = "token")] string Token)
    : ISwHashRequest, IRequest<ISwResult<SwGetUserNameRequest.SwUserNameResponse>>
{
    public class Handler : IRequestHandler<SwGetUserNameRequest, ISwResult<SwUserNameResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<ISwResult<SwUserNameResponse>> Handle(
            SwGetUserNameRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.GetBalanceAsync(
                request.Token,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToSwResult<SwUserNameResponse>();
            var data = walletResult.Data;

            var response = new SwUserNameResponse(
                data.UserId,
                data.Username);

            return SwResultFactory.Success(response);
        }
    }

    public record SwUserNameResponse(int UserId, string UserName);
}