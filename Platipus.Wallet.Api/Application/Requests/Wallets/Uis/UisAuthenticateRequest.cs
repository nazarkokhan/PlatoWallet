#pragma warning disable CS8618
namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis;

using System.Xml.Serialization;
using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Uis;
using Results.Uis.WithData;

[XmlRoot("REQUEST")]
public class UisAuthenticateRequest : IUisHashRequest, IRequest<IUisResult<UisResponseContainer>>
{
    [XmlElement("TOKEN")]
    public string Token { get; set; }

    [XmlElement("HASH")]
    public string Hash { get; set; }

    public class Handler : IRequestHandler<UisAuthenticateRequest, IUisResult<UisResponseContainer>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IUisResult<UisResponseContainer>> Handle(
            UisAuthenticateRequest request,
            CancellationToken cancellationToken)
        {
            var token = new Guid(request.Token);
            var user = await _context.Set<User>()
                .Where(u => u.Sessions.Any(s => s.Id == token))
                .Select(
                    u => new
                    {
                        u.Id,
                        u.UserName,
                        u.Balance,
                        CurrencyName = u.Currency.Name,
                        u.IsDisabled,
                        Sessions = u.Sessions
                            .Where(s => s.ExpirationDate > DateTime.UtcNow)
                            .Select(
                                s => new
                                {
                                    s.Id,
                                    s.ExpirationDate
                                })
                            .ToList()
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null || user.IsDisabled)
                return UisResultFactory.Failure<UisResponseContainer>(UisErrorCode.ExpiredToken);

            if (user.Sessions.All(s => s.Id != token))
                return UisResultFactory.Failure<UisResponseContainer>(UisErrorCode.ExpiredToken);

            var response = new UisAuthenticateResponse
            {
                UserId = user.UserName,
                Currency = user.CurrencyName,
                Balance = user.Balance
            };

            var container = new UisResponseContainer
            {
                Request = request,
                Time = DateTime.UtcNow,
                Response = response
            };
            return UisResultFactory.Success(container);
        }
    }

    public string GetSource()
    {
        return Token;
    }

    [XmlRoot("RESPONSE")]
    public record UisAuthenticateResponse
    {
        [XmlElement("RESULT")]
        public string Result { get; set; } = "OK";

        [XmlElement("USERID")]
        public string UserId { get; set; }

        [XmlElement("USERNAME")]
        public string? Username { get; set; }

        [XmlElement("FIRSTNAME")]
        public string? Firstname { get; set; }

        [XmlElement("LASTNAME")]
        public string? Lastname { get; set; }

        [XmlElement("EMAIL")]
        public string? Email { get; set; }

        [XmlElement("CURRENCY")]
        public string Currency { get; set; }

        [XmlElement("BALANCE")]
        public decimal Balance { get; set; }
    }
}