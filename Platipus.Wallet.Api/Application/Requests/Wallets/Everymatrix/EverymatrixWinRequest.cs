namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using Base;
using Base.Response;
using Domain.Entities;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Everymatrix;
using Results.Everymatrix.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using Services.Wallet.DTOs;

// public record EverymatrixWinRequest(
//     string SupplierUser,
//     string TransactionUuid,
//     Guid Token,
//     bool RoundClosed,
//     string Round,
//     string? RewardUuid,
//     string RequestUuid,
//     string ReferenceTransactionUuid,
//     bool IsFree,
//     int GameId,
//     string GameCode,
//     string Currency,
//     string? Bet,
//     int Amount,
//     EverymatrixMetaDto? Meta) : IEverymatrixBaseRequest, IRequest<IEverymatrixResult<EverymatrixBalanceResponse>>
// {
//     public class Handler : IRequestHandler<EverymatrixWinRequest, IEverymatrixResult<EverymatrixBalanceResponse>>
//     {
//         private readonly IWalletService _wallet;
//
//         public Handler(IWalletService wallet)
//         {
//             _wallet = wallet;
//         }
//
//         public async Task<IEverymatrixResult<EverymatrixBalanceResponse>> Handle(
//             EverymatrixWinRequest request,
//             CancellationToken cancellationToken)
//         {
//             var walletRequest = request.Map(
//                 r => new WinRequest(
//                     r.Token,
//                     r.SupplierUser,
//                     r.Currency,
//                     r.GameCode,
//                     r.Round,
//                     r.TransactionUuid,
//                     r.RoundClosed,
//                     r.Amount / 100000m));
//
//             var walletResult = await _wallet.WinAsync(walletRequest, cancellationToken);
//             if (walletResult.IsFailure)
//                 return walletResult.ToEverymatrixResult<EverymatrixBalanceResponse>();
//
//             var response = walletResult.Data.Map(
//                 d => new EverymatrixBalanceResponse(
//                     (int)(d.Balance * 100000),
//                     request.SupplierUser,
//                     request.RequestUuid,
//                     d.Currency));
//
//             return EverymatrixResultFactory.Success(response);
//         }
//     }
// }
//
// public record AuthenticateEverymatrixRequest(
//     string LaunchToken,
//     string RequestScope) : IEverymatrixBaseRequest, IRequest<IEverymatrixResult<AuthenticateEverymatrixRequest.Response>>
// {
//     public class Handler : IRequestHandler<AuthenticateEverymatrixRequest, IEverymatrixResult<Response>>
//     {
//         private readonly WalletDbContext _context;
//
//         public Handler(WalletDbContext context)
//         {
//             _context = context;
//         }
//
//         public async Task<IEverymatrixResult<Response>> Handle(
//             AuthenticateEverymatrixRequest request,
//             CancellationToken cancellationToken)
//         {
//             var user = await _context.Set<User>()
//                 .Where(u => u.Sessions.Any(s => s.Id == new Guid(request.LaunchToken)))
//                 .Select(
//                     u => new
//                     {
//                         u.Id,
//                         u.Balance,
//                         Currency = u.Currency.Name,
//                         u.UserName,
//                         u.SwUserId
//                     })
//                 .FirstOrDefaultAsync(cancellationToken: cancellationToken);
//
//             if (user is null)
//                 return EverymatrixResultFactory.Failure<Response>(EverymatrixErrorCode.InvalidHash);
//
//             var session = new Session()
//             {
//                 Id = Guid.NewGuid(),
//                 UserId = user.Id,
//                 ExpirationDate = DateTime.UtcNow.AddDays(1)
//             };
//
//             _context.Add(session);
//             await _context.SaveChangesAsync(cancellationToken);
//
//             var response = new Response(
//                 session.Id.ToString(),
//                 user.Balance,
//                 user.Currency,
//                 user.UserName,
//                 user.SwUserId!.Value);
//             return EverymatrixResultFactory.Success(response);
//         }
//     }
//
//     public record Response(
//         string Token,
//         decimal TotalBalance,
//         string Currency,
//         string UserName,
//         int UserId,
//         string Country = "ua",
//         int Age = 26,
//         string Sex = "male",
//         string Status = "Ok");
// }