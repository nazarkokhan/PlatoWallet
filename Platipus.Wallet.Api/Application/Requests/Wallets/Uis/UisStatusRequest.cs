namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis;

using System.Xml.Serialization;
using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Results.Uis;
using Results.Uis.WithData;

#pragma warning disable CS8618
[XmlRoot("REQUEST")]
public class UisStatusRequest : IUisHashRequest, IRequest<IUisResult<UisResponseContainer>>
{
    [XmlElement("USERID")]
    [BindProperty(Name = "userId")]
    public string UserId { get; set; }

    [XmlElement("CPTRANSACTIONID")]
    [BindProperty(Name = "CPTransactionID")]
    public string CpTransactionId { get; set; }

    [XmlElement("HASH")]
    [BindProperty(Name = "hash")]
    public string? Hash { get; set; }

    public class Handler : IRequestHandler<UisStatusRequest, IUisResult<UisResponseContainer>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IUisResult<UisResponseContainer>> Handle(
            UisStatusRequest request,
            CancellationToken cancellationToken)
        {
            var transaction = await _context.Set<Transaction>()
                .Where(u => u.Id == request.CpTransactionId)
                .Select(
                    u => new
                    {
                        u.Id,
                        u.InternalId
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (transaction is null)
                return UisResultFactory.Failure<UisResponseContainer>(UisErrorCode.ExpiredToken);

            var response = new UisStatusResponse { UisSystemTransactionId = transaction.InternalId };

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
        return UserId + CpTransactionId;
    }

    [XmlRoot("RESPONSE")]
    public record UisStatusResponse
    {
        [XmlElement("RESULT")]
        public string Result { get; set; } = "OK";

        [XmlElement("UISSYSTEMTRANSACTIONID")]
        public string UisSystemTransactionId { get; set; }
    }
}