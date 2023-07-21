namespace Platipus.Wallet.Api.Application.Services.EvenbetGamesApi.External;

using Application.Requests.Wallets.Evenbet.Base;using Domain.Entities;
using FluentValidation;
using Helpers.Common;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Responses.Evenbet;
using Results.Evenbet;
using Results.Evenbet.WithData;

public sealed record EvenbetLoginRequest
    ([property: JsonProperty("token")] string Token) : IEvenbetRequest, IRequest<IEvenbetResult<EvenbetLoginResponse>>
{
    public sealed class Handler : IRequestHandler<EvenbetLoginRequest, IEvenbetResult<EvenbetLoginResponse>>
    {
        private readonly WalletDbContext _walletDbContext;

        public Handler(WalletDbContext walletDbContext)
        {
            _walletDbContext = walletDbContext;
        }

        public async Task<IEvenbetResult<EvenbetLoginResponse>> Handle(
            EvenbetLoginRequest request,
            CancellationToken cancellationToken)
        {
            var loginData = await _walletDbContext.Set<Session>()
               .TagWith("Login")
               .Where(s => s.Id == request.Token)
               .Select(
                    d => new
                    {
                        Token = d.Id,
                        d.User.Balance,
                        Currency = d.User.Currency.Id,
                        Nickname = d.User.Username,
                        UserId = d.User.Id
                    })
               .FirstOrDefaultAsync(cancellationToken);

            if (loginData is null)
            {
                return EvenbetResultFactory.Failure<EvenbetLoginResponse>(EvenbetErrorCode.INVALID_TOKEN);
            }

            var response = new EvenbetLoginResponse(
                loginData.Token,
                (int)MoneyHelper.ConvertToCents(loginData.Balance),
                loginData.Currency,
                loginData.Nickname,
                DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                loginData.UserId);

            return EvenbetResultFactory.Success(response);
        }
    }

    public sealed class EvenbetLoginRequestValidator : AbstractValidator<EvenbetLoginRequest>
    {
        public EvenbetLoginRequestValidator()
        {
            RuleFor(x => x.Token)
               .NotEmpty()
               .WithMessage("Token is required");
        }
    }
}