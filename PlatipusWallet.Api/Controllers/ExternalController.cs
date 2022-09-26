namespace PlatipusWallet.Api.Controllers;

using Abstract;
using Application.Requests.Base.Responses;
using Application.Requests.DTOs;
using Application.Requests.External;
using Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("external")]
public class ExternalController : ApiController
{
    private readonly IMediator _mediator;

    public ExternalController(IMediator mediator) => _mediator = mediator;

    [HttpPost("signup")]
    [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> SignUp(SignUpRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("login")]
    [ProducesResponseType(typeof(LogInRequest.Response), StatusCodes.Status200OK)]
    public async Task<IActionResult> LogIn(LogInRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("add-balance")]
    [ProducesResponseType(typeof(LogInRequest.Response), StatusCodes.Status200OK)]
    public async Task<IActionResult> LogIn(AddBalanceRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("casino-currencies")]
    [ProducesResponseType(typeof(List<GetCurrencyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CasinoCurrencies(GetCasinoCurrenciesRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("round")]
    [ProducesResponseType(typeof(BalanceResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CasinoCurrencies(AddRoundRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}