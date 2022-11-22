namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.DTOs;
using Application.Requests.External;
using Application.Requests.Wallets.Psw.Base.Response;
using Extensions;
using Microsoft.AspNetCore.Mvc;

[Route("external")]
public class ExternalController : ApiController
{
    private readonly IMediator _mediator;

    public ExternalController(IMediator mediator)
        => _mediator = mediator;

    [HttpPost("signup")]
    [ProducesResponseType(typeof(PswBaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> SignUp(SignUpRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("login")]
    [ProducesResponseType(typeof(LogInRequest.Response), StatusCodes.Status200OK)]
    public async Task<IActionResult> LogIn(LogInRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPut("add-balance")]
    [ProducesResponseType(typeof(LogInRequest.Response), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddBalance(AddBalanceRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("casino-currencies")]
    [ProducesResponseType(typeof(List<GetCurrencyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CasinoCurrencies(
        [FromQuery] GetCasinoCurrenciesRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("round")]
    [ProducesResponseType(typeof(PswBalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Round(AddRoundRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}