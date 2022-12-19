namespace Platipus.Wallet.Api.Controllers;

// [Route("wallet/uis")]
// [MockedErrorActionFilter(Order = 1)]
// [Hub88VerifySignatureFilter(Order = 2)]
// [JsonSettingsName(nameof(CasinoProvider.Hub88))]
// [Produces(MediaTypeNames.Application.Xml)]
// [Consumes(MediaTypeNames.Application.Xml)]
// public class WalletUisController : Controller
// {
//     private readonly IMediator _mediator;
//
//     public WalletUisController(IMediator mediator)
//         => _mediator = mediator;
//
//     [HttpPost("user/balance")]
//     [ProducesResponseType(typeof(Hub88BalanceResponse), StatusCodes.Status200OK)]
//     public async Task<IActionResult> Balance(
//         [FromHeader(Name = Hub88Headers.XHub88Signature)] string sign,
//         Hub88GetBalanceRequest request,
//         CancellationToken cancellationToken)
//         => (await _mediator.Send(request, cancellationToken)).ToActionResult();
//
//     [HttpPost("transaction/bet")]
//     [ProducesResponseType(typeof(Hub88BalanceResponse), StatusCodes.Status200OK)]
//     public async Task<IActionResult> Bet(
//         [FromHeader(Name = Hub88Headers.XHub88Signature)] string sign,
//         Hub88BetRequest request,
//         CancellationToken cancellationToken)
//         => (await _mediator.Send(request, cancellationToken)).ToActionResult();
//
//     [HttpPost("transaction/win")]
//     [ProducesResponseType(typeof(Hub88BalanceResponse), StatusCodes.Status200OK)]
//     public async Task<IActionResult> Win(
//         [FromHeader(Name = Hub88Headers.XHub88Signature)] string sign,
//         Hub88WinRequest request,
//         CancellationToken cancellationToken)
//         => (await _mediator.Send(request, cancellationToken)).ToActionResult();
//
//     [HttpPost("transaction/rollback")]
//     [ProducesResponseType(typeof(Hub88BalanceResponse), StatusCodes.Status200OK)]
//     public async Task<IActionResult> Rollback(
//         [FromHeader(Name = Hub88Headers.XHub88Signature)] string sign,
//         Hub88RollbackRequest request,
//         CancellationToken cancellationToken)
//         => (await _mediator.Send(request, cancellationToken)).ToActionResult();
// }