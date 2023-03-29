namespace Platipus.Wallet.Api.Application.Requests.Test;

using System.ComponentModel;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Xml;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record CreateErrorMockRequest(
    [property: DefaultValue("reevo_nazar")] string Username,
    MockedErrorMethod Method,
    List<CreateErrorMockRequest.CreateErrorMockItem> Items) : IRequest<IResult>
{
    public class Handler : IRequestHandler<CreateErrorMockRequest, IResult>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult> Handle(
            CreateErrorMockRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .TagWith("GetUserWithMockedError")
                .Where(e => e.Username == request.Username)
                .Include(u => u.MockedErrors.Where(m => m.Method == request.Method))
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure(ErrorCode.BadParametersInTheRequest);

            var mockedErrors = user.MockedErrors;

            if (mockedErrors.Count is not 0)
                return ResultFactory.Failure(ErrorCode.ErrorMockAlreadyExists);

            for (var i = 0; i < request.Items.Count; i++)
            {
                var item = request.Items[i];
                var contentType = item.ContentType switch
                {
                    "text" => MediaTypeNames.Text.Plain,
                    "xml" => MediaTypeNames.Text.Xml,
                    "html" => MediaTypeNames.Text.Html,
                    "json" or _ => MediaTypeNames.Application.Json
                };

                var body = item.Body;
                switch (contentType)
                {
                    case MediaTypeNames.Application.Json:
                        try
                        {
                            body = JsonDocument.Parse(body).RootElement.ToString();
                        }
                        catch (Exception e)
                        {
                            return ResultFactory.Failure(ErrorCode.InvalidJsonContent, e);
                        }

                        break;
                    case MediaTypeNames.Text.Xml:
                        try
                        {
                            var doc = new XmlDocument();
                            doc.LoadXml(body);
                            body = doc.OuterXml;
                        }
                        catch (Exception e)
                        {
                            return ResultFactory.Failure(ErrorCode.InvalidXmlContent, e);
                        }

                        break;
                }

                if (item.Timeout > TimeSpan.FromMinutes(3))
                    return ResultFactory.Failure(ErrorCode.MaxErrorMockTimeoutIs3Minutes);

                var mockedError = new MockedError
                {
                    Method = request.Method,
                    Body = body,
                    HttpStatusCode = item.HttpStatusCode,
                    ContentType = contentType,
                    Count = item.Count <= 0 ? 1 : item.Count,
                    ExecutionOrder = i + 1,
                    Timeout = item.Timeout,
                    UserId = user.Id
                };

                mockedErrors.Add(mockedError);
            }

            _context.AddRange(mockedErrors);
            await _context.SaveChangesAsync(cancellationToken);

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