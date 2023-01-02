namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using Base;
using Base.Response;
using Extensions;
using Results.Everymatrix;
using Results.Everymatrix.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using Services.Wallet.DTOs;

// public record EverymatrixRollbackRequest(
//     string SupplierUser,
//     string TransactionUuid,
//     Guid Token,
//     bool RoundClosed,
//     string Round,
//     string RequestUuid,
//     string ReferenceTransactionUuid,
//     int GameId,
//     string GameCode,
//     EverymatrixMetaDto? Meta) : IEverymatrixBaseRequest, IRequest<IEverymatrixResult<EverymatrixBalanceResponse>>
// {
//     public class Handler : IRequestHandler<EverymatrixRollbackRequest, IEverymatrixResult<EverymatrixBalanceResponse>>
//     {
//         private readonly IWalletService _wallet;
//
//         public Handler(IWalletService wallet)
//         {
//             _wallet = wallet;
//         }
//
//         public async Task<IEverymatrixResult<EverymatrixBalanceResponse>> Handle(
//             EverymatrixRollbackRequest request,
//             CancellationToken cancellationToken)
//         {
//             var walletRequest = request.Map(
//                 r => new RollbackRequest(
//                     r.Token,
//                     r.SupplierUser,
//                     r.GameCode,
//                     r.Round,
//                     r.TransactionUuid));
//
//             var walletResult = await _wallet.RollbackAsync(walletRequest, cancellationToken);
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