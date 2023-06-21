namespace Platipus.Wallet.Api.Controllers.Other;

using Microsoft.AspNetCore.Mvc;
using Application.Requests.Wallets.EmaraPlay;
using Application.Requests.Wallets.EmaraPlay.Responses;
using Abstract;
using Extensions;


[Route("wallet/emara-play")]
public sealed class ExternalEmaraplayController : RestApiController
{
    private readonly IMediator _mediator;

    public ExternalEmaraplayController(IMediator mediator) => 
        _mediator = mediator;

    /// <summary>
    ///     Gets round's details.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     <see cref="EmaraPlayGetRoundDetailsResponse"/>.
    /// </returns>
    [HttpPost("round-details")]
    [ProducesResponseType(typeof(EmaraPlayGetRoundDetailsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoundDetails(
        [FromBody] EmaraPlayGetRoundDetailsRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();


    /// <summary>
    ///  Award endpoint will allow the operator to grant some free spins to a user with some
    ///  feature such as game list, min bet, etc.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("award")]
    [ProducesResponseType(typeof(EmaraPlayAwardResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Award(
        [FromBody] EmaraPlayAwardRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();


    /// <summary>
    ///     Cancel endpoint will allow the operator to cancel an awarded free spin campaign to a
    ///     user.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    ///     <see cref="EmaraPlayCancelResponse"/>.
    /// </returns>
    [HttpPost("cancel")]
    [ProducesResponseType(typeof(EmaraPlayCancelResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancel(
        [FromBody] EmaraPlayCancelRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}