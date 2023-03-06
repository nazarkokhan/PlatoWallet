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
            var user = await _context.Set<MockedError>()
                .TagWith("GetMockedErrors")
                .Where(e => e.User.Username == request.Username)
                .Where(e => request.Method == null || e.Method == request.Method)
                .Select(e => new
                {
                    e.Method
                })
                .ToListAsync(cancellationToken);

            // if (user is null)
            //     return ResultFactory.Failure(ErrorCode.BadParametersInTheRequest);
            //
            // var mockedErrors = user.MockedErrors;
            //
            // if (mockedErrors.Count is not 0)
            //     return ResultFactory.Failure(ErrorCode.ErrorMockAlreadyExists);
            //
            // foreach (var mock in request.Items)
            // {
            //     var contentType = mock.ContentType switch
            //     {
            //         "text" => MediaTypeNames.Text.Plain,
            //         "xml" => MediaTypeNames.Text.Xml,
            //         "html" => MediaTypeNames.Text.Html,
            //         "json" or _ => MediaTypeNames.Application.Json
            //     };
            //
            //     if (mock.Timeout > TimeSpan.FromMinutes(3))
            //         return ResultFactory.Failure(ErrorCode.MaxTimeout3Mins);
            //
            //     var mockedError = new MockedError
            //     {
            //         Method = request.Method,
            //         Body = mock.Body,
            //         HttpStatusCode = mock.HttpStatusCode,
            //         ContentType = contentType,
            //         Count = mock.Count <= 0 ? 1 : mock.Count,
            //         Timeout = mock.Timeout,
            //         UserId = user.Id
            //     };
            //
            //     mockedErrors.Add(mockedError);
            // }
            //
            // _context.AddRange(mockedErrors);
            // await _context.SaveChangesAsync(cancellationToken);

            return ResultFactory.Success();
        }
    }

    public record CreateErrorMockItem(
        [property: DefaultValue("your_json_or_xml")] string Body,
        [property: DefaultValue(200)] HttpStatusCode HttpStatusCode,
        [property: DefaultValue("json")] string ContentType,
        [property: DefaultValue(1)] int Count,
        [property: DefaultValue("00:00:10")] TimeSpan? Timeout);
}