namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis;

using System.Xml.Serialization;
using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Uis;
using Results.Uis.WithData;

#pragma warning disable CS8618
[XmlRoot("REQUEST")]
public record UisGetBalanceRequest : IUisHashRequest, IRequest<IUisResult<UisResponseContainer>>
{
    [XmlElement("USERID")]
    public string UserId { get; init; }

    [XmlElement("HASH")]
    public string Hash { get; init; }

    public class Handler : IRequestHandler<UisGetBalanceRequest, IUisResult<UisResponseContainer>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IUisResult<UisResponseContainer>> Handle(
            UisGetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var transaction = await _context.Set<User>()
                .Where(u => u.UserName == request.UserId)
                .Select(
                    u => new
                    {
                        u.Id,
                        u.Balance
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (transaction is null)
                return UisResultFactory.Failure<UisResponseContainer>(UisErrorCode.ExpiredToken);

            var response = new UisGetBalanceResponse { Balance = transaction.Balance };

            var container = new UisResponseContainer
            {
                Request = request,
                Response = response
            };
            return UisResultFactory.Success(container);
        }
    }

    public string GetSource()
    {
        return UserId;
    }

    [XmlRoot("RESPONSE")]
    public record UisGetBalanceResponse
    {
        [XmlElement("RESULT")]
        public string Result { get; set; } = "OK";

        [XmlElement("BALANCE")]
        public decimal Balance { get; set; }
    }
}