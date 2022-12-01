namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sw;

using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Results.Sw;
using Results.Sw.WithData;
using Services.Wallet;

public record SwGetUserNameRequest(
        [property: BindProperty(Name = "providerid")] int ProviderId,
        [property: BindProperty(Name = "userid")] int UserId,
        [property: BindProperty(Name = "hash")] string Hash,
        [property: BindProperty(Name = "amount")] Guid Token)
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
            var user = await _context.Set<User>()
                .Where(u => u.SwUserId == request.UserId && u.Casino.SwProviderId == request.ProviderId)
                .Select(
                    u => new
                    {
                        u.UserName,
                        u.SwUserId
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return SwResultFactory.Failure<SwUserNameResponse>(SwErrorCode.UserNotFound);

            var response = new SwUserNameResponse(request.UserId, user.UserName);

            return SwResultFactory.Success(response);
        }
    }

    public record SwUserNameResponse(int UserId, string UserName);
}