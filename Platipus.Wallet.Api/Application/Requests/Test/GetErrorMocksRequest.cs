namespace Platipus.Wallet.Api.Application.Requests.Test;

using System.ComponentModel;
using System.Net;
using System.Net.Mime;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record GetErrorMocksRequest(
    [property: DefaultValue("reevo_nazar")] string Username,
    [property: DefaultValue(null)] MockedErrorMethod? Method) : IRequest<IResult>
{
    public class Handler : IRequestHandler<GetErrorMocksRequest, IResult>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }


        public async Task<IResult> Handle(
            GetErrorMocksRequest request,
            CancellationToken cancellationToken)
        {
            var errorMocks = await _context.Set<MockedError>()
                .TagWith("GetMockedErrors")
                .Where(e => e.User.Username == request.Username)
                .Where(e => request.Method == null || e.Method == request.Method)
                .ToListAsync(cancellationToken);

            var responseItems = errorMocks
                .GroupBy(e => e.Method)
                .Select(
                    e => new Dictionary<MockedErrorMethod, GetMockedErrorItem[]>
                    {
                        {
                            e.Key, e
                                .OrderBy(g => g.ExecutionOrder)
                                .Select(
                                    g => new GetMockedErrorItem(
                                        g.Body,
                                        g.HttpStatusCode,
                                        g.ContentType,
                                        g.Count,
                                        g.ExecutionOrder,
                                        g.Timeout))
                                .ToArray()
                        }
                    })
                .ToList();

            var response = new ResponseDto(responseItems);

            return ResultFactory.Success(response);
        }
    }

    public record ResponseDto(List<Dictionary<MockedErrorMethod, GetMockedErrorItem[]>> Items);

    public record GetMockedErrorItem(
        string Body,
        HttpStatusCode HttpStatusCode,
        string ContentType,
        int Count,
        int ExecutionOrder,
        TimeSpan? Timeout);
}