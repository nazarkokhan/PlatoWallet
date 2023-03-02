// // ReSharper disable IdentifierTypo
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
// public record PariMatchCancelRequest(
//     string Cid,
//     string PlayerId,
//     string ProductId,
//     string TxId,
//     string RoundId,
//     int Amount,
//     string Currency) : IRequest<IPariMatchResult<PariMatchCancelRequest.PariMatchCancelResponse>>
// {
//     public record PariMatchCancelResponse(
//         DateTime CreatedAt,
//         int Balance,
//         string Txid,
//         string ProcessedTxId,
//         bool AlreadyProcessed);
//
//     public class Handler : IRequestHandler<PariMatchCancelRequest, IPariMatchResult<PariMatchCancelResponse>>
//     {
//         private readonly WalletDbContext _context;
//
//
//         public Handler(WalletDbContext dbContext)
//         {
//             _context = dbContext;
//         }
//
//         public async Task<IPariMatchResult<PariMatchCancelResponse>> Handle(
//             PariMatchCancelRequest request,
//             CancellationToken cancellationToken)
//         {
//             var user = await _context.Set<User>()
//                 .FirstOrDefaultAsync(u => u.Id == new Guid(request.PlayerId), cancellationToken: cancellationToken);
//
//             if (user is null)
//             {
//                 return Failure<PariMatchCancelResponse>(
//                     PariMatchErrorCode.IntegrationHubFailure,
//                     new Exception("User isn;t found"));
//             }
//
//             if (user.IsDisabled)
//             {
//                 return Failure<PariMatchCancelResponse>(PariMatchErrorCode.LockedPlayer);
//             }
//
//             var round = await _context.Set<Round>()
//                 .Where(
//                     r => r.Id == request.RoundId
//                       && r.UserId == new Guid(request.PlayerId))
//                 .Include(r => r.User.Currency)
//                 .Include(r => r.Transactions)
//                 .FirstOrDefaultAsync(cancellationToken);
//
//             if (round is null)
//                 return Failure<PariMatchCancelResponse>(
//                     PariMatchErrorCode.IntegrationHubFailure,
//                     new Exception("Round isn't found"));
//
//             if (round.Finished)
//                 return Failure<PariMatchCancelResponse>(
//                     PariMatchErrorCode.IntegrationHubFailure,
//                     new Exception("Round is finished"));
//
//             var transaction = await _context.Set<Transaction>()
//                 .Where(t => t.Id == request.TxId)
//                 .FirstOrDefaultAsync(cancellationToken);
//
//             if (transaction is null)
//             {
//                 return Failure<PariMatchCancelResponse>(PariMatchErrorCode.InvalidTransactionId);
//             }
//
//             user.Balance -= transaction.Amount;
//
//             _context.Remove(transaction);
//             _context.Update(user);
//             await _context.SaveChangesAsync();
//
//             var response = new PariMatchCancelResponse(
//                 transaction.CreatedDate,
//                 (int) user.Balance,
//                 transaction.Id,
//                 "TxId on Eva",
//                 false);
//
//             return Success(response);
//         }
//     }
// }

