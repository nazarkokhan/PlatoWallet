namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Everymatrix;
using Results.Everymatrix.WithData;

public record EverymatrixRequestAuthenticateRequest(
    string LaunchToken,
    string RequestScope) : IRequest<IEverymatrixResult<EverymatrixRequestAuthenticateRequest.EveryMatrixAuthenticationResponse>>
{
    public class Handler : IRequestHandler<EverymatrixRequestAuthenticateRequest, IEverymatrixResult<EverymatrixRequestAuthenticateRequest.EveryMatrixAuthenticationResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IEverymatrixResult<EverymatrixRequestAuthenticateRequest.EveryMatrixAuthenticationResponse>> Handle(
            EverymatrixRequestAuthenticateRequest request,
            CancellationToken cancellationToken)
        {
            var requiredData = request.RequestScope.Split(", ").ToList();

            var user = await _context.Set<User>()
                .Where(u => u.Id == new Guid(request.LaunchToken))
                .Select(
                    u => new
                    {
                        u.Id,
                        u.Balance,
                        Currency = u.Currency.Name,
                        u.UserName,
                        u.SwUserId
                    })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (user is null)
            {
                return EverymatrixResultFactory.Failure<EverymatrixRequestAuthenticateRequest.EveryMatrixAuthenticationResponse>(
                    EverymatrixErrorCode.TokenNotFound,
                    new Exception("The launch token is not valid"));
            }

            var session = new Session()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ExpirationDate = DateTime.UtcNow.AddDays(1)
            };


            _context.Add(session);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new EveryMatrixAuthenticationResponse(
                session.Id.ToString(),
                user.Balance,
                user.Currency,
                user.UserName,
                Convert.ToString(user.Id)!);
            return EverymatrixResultFactory.Success(response);
        }
    }

    public record EveryMatrixAuthenticationResponse(
        string Token,
        decimal TotalBalance,
        string Currency,
        string UserName,
        string UserId,
        string Country = "ua",
        int Age = 26,
        string Sex = "male",
        string Status = "Ok");
}