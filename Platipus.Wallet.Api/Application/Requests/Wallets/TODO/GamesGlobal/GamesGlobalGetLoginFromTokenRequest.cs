// namespace Platipus.Wallet.Api.Application.Requests.Wallets.GamesGlobal;
//
// using Base;
// using Domain.Entities;
// using Horizon.XmlRpc.Core;
// using Infrastructure.Persistence;
// using Microsoft.EntityFrameworkCore;
// using Results.GamesGlobal;
// using Results.GamesGlobal.WithData;
//
// public record GamesGlobalGetLoginFromTokenRequest(Guid Token)
//     : IGamesGlobalBaseRequest, IRequest<IGamesGlobalResult<GamesGlobalGetLoginFromTokenRequest.Response>>
//
// {
//     public class Handler : IRequestHandler<GamesGlobalGetLoginFromTokenRequest, IGamesGlobalResult<Response>>
//     {
//         private readonly WalletDbContext _context;
//
//         public Handler(WalletDbContext context)
//         {
//             _context = context;
//         }
//
//         public async Task<IGamesGlobalResult<Response>> Handle(
//             GamesGlobalGetLoginFromTokenRequest request,
//             CancellationToken cancellationToken)
//         {
//             var user = await _context.Set<User>()
//                 .Where(u => u.Sessions.Select(s => s.Id).Contains(request.Token))
//                 .Select(
//                     u => new
//                     {
//                         u.UserName,
//                         Currency = u.Currency.Id,
//                         u.Balance,
//                         SwUserId = u.Id,
//                         Casino = new
//                         {
//                             Id = u.Casino.Id,
//                             SwProviderId = u.Casino.NumberId,
//                             u.Casino.Provider,
//                         }
//                     })
//                 .FirstOrDefaultAsync(cancellationToken);
//
//             if (user is null)
//                 return GamesGlobalResultFactory.Failure<Response>(GamesGlobalErrorCode.InvalidUserId);
//
//             var userInfo = new GamesGlobalUserInfoDto
//             {
//                 ServerId = user.Casino.SwProviderId ?? 0,
//                 UserId = user.SwUserId ?? 0,
//                 UserName = user.UserName
//             };
//
//             var response = new Response(
//                 userInfo,
//                 0,
//                 user.Currency,
//                 (long)(user.Balance * 100),
//                 0,
//                 "",
//                 new GamesGlobalPlayerGroupDto[]
//                 {
//                     new()
//                     {
//                         PlayerGroupName = "",
//                         Rank = 0,
//                         PlayerGroupId = ""
//                     }
//                 });
//
//             return GamesGlobalResultFactory.Success(response);
//         }
//     }
//
//     public record Response(
//         [property: XmlRpcMember("userInfo")] GamesGlobalUserInfoDto UserInfo,
//         [property: XmlRpcMember("userType")] int UserType,
//         [property: XmlRpcMember("currency")] string Currency,
//         [property: XmlRpcMember("cashBal")] long CashBal,
//         [property: XmlRpcMember("bonusBal")] long BonusBal,
//         [property: XmlRpcMember("playerInformationXML")] string PlayerInformationXml,
//         // [property: XmlRpcMember("gender")] string Gender,
//         // [property: XmlRpcMember("dateOfBirth")] string DateOfBirth,
//         // [property: XmlRpcMember("territoryOfOrigin")] GamesGlobalTerritoryOfOriginDto TerritoryOfOrigin,
//         [property: XmlRpcMember("playerGroups")] GamesGlobalPlayerGroupDto[] PlayerGroups);
// }