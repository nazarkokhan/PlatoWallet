namespace PlatipusWallet.Api.Application.Requests.External;

using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Base.Responses;
using Options;
using Results.Common;
using Results.Common.Result;
using Results.Common.Result.WithData;
using Domain.Entities;
using Infrastructure.Persistence;
using Results.Common.Result.Factories;
using Services.GamesApiService;

public record LogInRequest(
    string UserName,
    string Password,
    string CasinoId,
    string Game) : IRequest<IResult<LogInRequest.Response>>
{
    public class Handler : IRequestHandler<LogInRequest, IResult<Response>>
    {
        private readonly WalletDbContext _context;
        private readonly IGamesApiClient _gamesApiClient;
        private readonly ILogger<Handler> _logger;

        public Handler(WalletDbContext context, IGamesApiClient gamesApiClient,
            ILogger<Handler> logger)
        {
            _context = context;
            _gamesApiClient = gamesApiClient;
            _logger = logger;
        }

        public async Task<IResult<Response>> Handle(
            LogInRequest request,
            CancellationToken cancellationToken)
        {
            var casino = await _context.Set<Casino>()
                .Where(c => c.Id == request.CasinoId)
                .FirstOrDefaultAsync(cancellationToken);

            if (casino is null)
                return ResultFactory.Failure<Response>(ErrorCode.InvalidCasinoId);

            var user = await _context.Set<User>()
                .Where(
                    u => u.UserName == request.UserName &&
                         u.CasinoId == request.CasinoId)
                .Include(u => u.Casino)
                .Include(u => u.Currency)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure<Response>(ErrorCode.InvalidUser);

            if (user.IsDisabled)
                return ResultFactory.Failure<Response>(ErrorCode.UserDisabled);

            if (user.Password != request.Password)
                return ResultFactory.Failure<Response>(ErrorCode.Unknown);

            var session = new Session
            {
                ExpirationDate = DateTime.UtcNow.AddDays(1),
                User = user,
            };

            _context.Add(session);
            await _context.SaveChangesAsync(cancellationToken);

            var getGameLinkResult = await _gamesApiClient.GetGameLinkAsync(
                user.Casino.Id,
                session.Id,
                user.UserName,
                user.Currency.Name,
                request.Game,
                cancellationToken: cancellationToken);

            string launchUrl;
            if (getGameLinkResult.IsSuccess)
                launchUrl = getGameLinkResult.Data.LaunchUrl;
            else
            {
                _logger.LogWarning("Launch url not created: {GetGameLinkResult}", getGameLinkResult);
                launchUrl = string.Empty;
            }

            var result = new Response(session.Id, user.Balance, getGameLinkResult.Data?.LaunchUrl ?? "");

            return ResultFactory.Success(result);
        }
    }

    public record Response(
        Guid SessionId,
        decimal Balance,
        string LaunchUrl) : BalanceResponse(Balance);

    public class Validator : AbstractValidator<SignUpRequest>
    {
        public Validator(IOptions<SupportedCurrenciesOptions> currenciesOptions)
        {
            var currenciesOptionsValue = currenciesOptions.Value;

            RuleFor(
                x => currenciesOptionsValue.Fiat.Contains(x.Currency) ||
                     currenciesOptionsValue.Crypto.Contains(x.Currency));

            RuleFor(p => p.Password)
                .MinimumLength(6)
                .MaximumLength(8);

            RuleFor(p => p.Balance)
                .ScalePrecision(2, 38);
        }
    }
}