#pragma warning disable CS8618
namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis;

using System.Xml.Serialization;
using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Results.Uis;
using Results.Uis.WithData;

[XmlRoot("REQUEST")]
public class UisAuthenticateRequest : IUisRequest, IRequest<IUisResult<UisResponseContainer>>
{
    [XmlElement("TOKEN")]
    [BindProperty(Name = "token")]
    public string Token { get; set; }

    [XmlElement("HASH")]
    [BindProperty(Name = "hash")]
    public string? Hash { get; set; }

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
            var session = await _context.Set<Session>()
                .Where(s => s.Id == request.Token)
                .Select(
                    s => new
                    {
                        s.Id,
                        s.ExpirationDate,
                        User = new
                        {
                            s.User.Id,
                            s.User.Username,
                            s.User.Balance,
                            CurrencyName = s.User.Currency.Id,
                            s.User.IsDisabled,
                        }
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (session is null || session.ExpirationDate < DateTime.UtcNow)
                return UisResultFactory.Failure<UisResponseContainer>(UisErrorCode.InvalidToken);

            var user = session.User;
            if (user.IsDisabled)
                return UisResultFactory.Failure<UisResponseContainer>(UisErrorCode.InvalidToken);

            var response = new UisAuthenticateResponse
            {
                UserId = user.Username,
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