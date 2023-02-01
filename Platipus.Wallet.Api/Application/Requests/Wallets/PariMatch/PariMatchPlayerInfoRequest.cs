// // ReSharper disable NotAccessedPositionalProperty.Global
// namespace Platipus.Wallet.Api.Application.Requests.Wallets.PariMatch;
//
// using Domain.Entities;
// using Infrastructure.Persistence;
// using Microsoft.EntityFrameworkCore;
// using Results.PariMatch;
// using Results.PariMatch.WithData;
// using static Results.PariMatch.PariMatchResultFactory;
//
// public record PariMatchPlayerInfoRequest(string Cid, string SessionToken)
//     : IRequest<IPariMatchResult<PariMatchPlayerInfoRequest.PariMatchPlayerInfoResponse>>
// {
//     public record PariMatchPlayerInfoResponse(
//         string PlayerId,
//         int Balance,
//         string Currency,
//         string Country,
//         string DisplayName);
//
//
//     public class Handler : IRequestHandler<PariMatchPlayerInfoRequest, IPariMatchResult<PariMatchPlayerInfoResponse>>
//     {
//         private readonly WalletDbContext _context;
//
//         public Handler(WalletDbContext context)
//         {
//             _context = context;
//         }
//
//
//         public async Task<IPariMatchResult<PariMatchPlayerInfoResponse>> Handle(
//             PariMatchPlayerInfoRequest request,
//             CancellationToken cancellationToken)
//         {
//             var session = await _context.Set<Session>()
//                 .FirstOrDefaultAsync(s => s.Id == new Guid(request.SessionToken), cancellationToken: cancellationToken);
//
//             if (session is null)
//             {
//                 return Failure<PariMatchPlayerInfoResponse>(PariMatchErrorCode.InvalidSessionKey);
//             }
//
//             var user = await _context.Set<User>()
//                 .TagWith("GetUserBalance")
//                 .Where(u => u.Id == session.UserId)
//                 .Select(
//                     s => new
//                     {
//                         s.Id,
//                         s.Balance,
//                         s.IsDisabled,
//                         // s.Country,
//                         s.UserName,
//                         Currency = s.Currency.Name
//                     })
//                 .FirstOrDefaultAsync(cancellationToken);
//
//             if (user is null)
//             {
//                 return Failure<PariMatchPlayerInfoResponse>(PariMatchErrorCode.IntegrationHubFailure, new Exception("User isn't found"));
//             }
//
//
//             if (user.IsDisabled)
//                 return Failure<PariMatchPlayerInfoResponse>(PariMatchErrorCode.LockedPlayer);
//
//             var response = new PariMatchPlayerInfoResponse(
//                 user.Id.ToString(),
//                 (int) user.Balance,
//                 user.Currency,
//                 "Need to add country to db",
//                 user.UserName);
//
//             return Success(response);
//         }
//     }
// }

