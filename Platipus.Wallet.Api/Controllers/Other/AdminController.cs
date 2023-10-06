namespace Platipus.Wallet.Api.Controllers.Other;

using Abstract;
using Application.Requests.Admin;
using Extensions;
using Microsoft.AspNetCore.Mvc;

[Route("admin")]
public sealed class AdminController : RestApiController
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator) => _mediator = mediator;

    [HttpGet("environment/page")]
    public async Task<IActionResult> GetEnvironments(CancellationToken cancellationToken)
        => (await _mediator.Send(new GetGameEnvironmentsPageRequest(), cancellationToken)).ToActionResult();

    [HttpPost("environment")]
    public async Task<IActionResult> CreateEnvironment(
        CreateGameEnvironmentRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPut("environment")]
    public async Task<IActionResult> UpdateEnvironment(
        UpdateGameEnvironmentRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("casino/page")]
    public async Task<IActionResult> GetCasinosPage(
        [FromQuery] GetCasinosPageRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpPost("casino")]
    public async Task<IActionResult> CreateCasino(CreateCasinoRequest request, CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("user/page")]
    public async Task<IActionResult> GetUsersPage(
        [FromQuery] GetUsersPageRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpGet("integrations/info")]
    [ProducesResponseType(typeof(List<GetWalletProvidersInfoRequest.WalletProviderInfo>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetIntegrationsInfo(CancellationToken cancellationToken)
        => (await _mediator.Send(new GetWalletProvidersInfoRequest(), cancellationToken)).ToActionResult();

    [HttpGet("currencies")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrenciesList(CancellationToken cancellationToken)
        => (await _mediator.Send(new GetCurrenciesListRequest(), cancellationToken)).ToActionResult();

    [HttpGet("currencies/{casinoId}")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrenciesByCasino(
        [FromRoute] string casinoId,
        CancellationToken cancellationToken)
        => (await _mediator.Send(new GetCurrenciesByCasinoRequest(casinoId), cancellationToken)).ToActionResult();

    [HttpPost("currencies")]
    [ProducesResponseType(typeof(AddCurrenciesToCasinoRequest.AddCurrenciesToCasinoResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddCurrenciesToCasino(
        [FromBody] AddCurrenciesToCasinoRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();

    [HttpDelete("currencies")]
    [ProducesResponseType(typeof(RemoveCurrenciesFromCasinoRequest.RemoveCurrenciesFromCasinoResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveCurrenciesFromCasino(
        [FromBody] RemoveCurrenciesFromCasinoRequest request,
        CancellationToken cancellationToken)
        => (await _mediator.Send(request, cancellationToken)).ToActionResult();
}