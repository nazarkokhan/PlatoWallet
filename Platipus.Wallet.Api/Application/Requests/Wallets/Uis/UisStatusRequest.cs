namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis;

using System.Xml.Serialization;
using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Results.Uis;
using Results.Uis.WithData;

[XmlRoot("REQUEST")]
public class UisStatusRequest
    : IUisUserIdRequest, IRequest<IUisResult<UisResponseContainer<UisStatusRequest, UisStatusRequest.UisStatusResponse>>>
{
    [XmlElement("USERID")]
    [BindProperty(Name = "userId")]
    public string UserId { get; set; } = null!;

    [XmlElement("CPTRANSACTIONID")]
    [BindProperty(Name = "CPTransactionID")]
    public string CpTransactionId { get; set; } = null!;

    [XmlElement("HASH")]
    [BindProperty(Name = "hash")]
    public string? Hash { get; set; }

    public class Handler
        : IRequestHandler<UisStatusRequest, IUisResult<UisResponseContainer<UisStatusRequest, UisStatusResponse>>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IUisResult<UisResponseContainer<UisStatusRequest, UisStatusResponse>>> Handle(
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
                return UisResultFactory.Failure<UisResponseContainer<UisStatusRequest, UisStatusResponse>>(
                    UisErrorCode.UnknownTransactionIdOrWasAlreadyProcessed);

            var response = new UisStatusResponse { UisSystemTransactionId = transaction.InternalId };

            var container = new UisResponseContainer<UisStatusRequest, UisStatusResponse>(request, response);

            return UisResultFactory.Success(container);
        }
    }

    public string GetSource()
    {
        return UserId + CpTransactionId;
    }

    public record UisStatusResponse : UisBaseResponse
    {
        [XmlElement("UISSYSTEMTRANSACTIONID")]
        public string UisSystemTransactionId { get; set; } = null!;
    }
}