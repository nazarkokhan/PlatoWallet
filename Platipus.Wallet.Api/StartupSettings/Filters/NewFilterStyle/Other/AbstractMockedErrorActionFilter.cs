namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle.Other;

using System.Net.Mime;
using System.Text.Json;
using LazyCache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Requests.Base;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Infrastructure.Persistence;

public abstract class AbstractMockedErrorActionFilter : IAsyncActionFilter
{
    private readonly ILogger<AbstractMockedErrorActionFilter> _logger;

    protected AbstractMockedErrorActionFilter(ILogger<AbstractMockedErrorActionFilter> logger)
    {
        _logger = logger;
    }

    protected abstract MockedErrorIdentifiers? GetMockedErrorIdentifiers(IBaseWalletRequest baseRequest);

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        _logger.LogDebug("Handling request with possible mocked error");

        var executedContext = await next();

        var walletRequest = context.ActionArguments.Values
            .OfType<IBaseWalletRequest>()
            .Single();

        var mockedErrorIdentifiers = GetMockedErrorIdentifiers(walletRequest);

        if (mockedErrorIdentifiers is null)
        {
            _logger.LogInformation("ErrorMockMethod not found");
            return;
        }

        var httpContext = context.HttpContext;
        var services = httpContext.RequestServices;

        var cache = services.GetRequiredService<IAppCache>();

        var (walletMethod, usernameOrSession, _) = mockedErrorIdentifiers.Value;

        var concurrencyKey = $"em:{usernameOrSession}:{walletMethod.ToString()}";
        var semaphoreSlim = cache.GetOrAdd(concurrencyKey, () => new SemaphoreSlim(1), TimeSpan.FromHours(1));

        try
        {
            await semaphoreSlim.WaitAsync();


            await Method(
                executedContext,
                mockedErrorIdentifiers.Value,
                concurrencyKey);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Mocking error failed");
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private async Task Method(
        ActionExecutedContext executedContext,
        MockedErrorIdentifiers mockedErrorIdentifiers,
        string concurrencyKey)
    {
        var httpContext = executedContext.HttpContext;

        var dbContext = httpContext.RequestServices.GetRequiredService<WalletDbContext>();

        var (walletMethod, usernameOrSession, callerIdentifiedBySession) = mockedErrorIdentifiers;

        var mockedErrorQuery = dbContext.Set<MockedError>()
            .Where(e => e.Method == walletMethod)
            .Where(
                e => callerIdentifiedBySession
                    ? e.User.Sessions.Any(s => s.Id == usernameOrSession)
                    : e.User.Username == usernameOrSession);

        var mockedError = await mockedErrorQuery
            .OrderBy(e => e.ExecutionOrder)
            .FirstOrDefaultAsync();

        if (mockedError is null)
        {
            _logger.LogDebug("Mocked error not found");
            return;
        }

        _logger.LogInformation(
            "Executing mocked error {@MockedError}",
            new
            {
                mockedError.Id,
                ExecutionConcurrencyKey = concurrencyKey,
                SessionOrUsername = usernameOrSession,
                CallerIdentifiedBySession = callerIdentifiedBySession,
                mockedError.Method,
                mockedError.Body,
                mockedError.HttpStatusCode,
                mockedError.ContentType,
                mockedError.Count,
                mockedError.ExecutionOrder,
                mockedError.UserId,
                mockedError.Timeout
            });

        if (mockedError.Timeout is not null)
            await Task.Delay(mockedError.Timeout.Value);

        const string responseItem = "response";

        switch (mockedError.ContentType)
        {
            case MediaTypeNames.Application.Json:
            {
                object? response = null;
                try
                {
                    response = JsonDocument.Parse(mockedError.Body);
                    httpContext.Items.Add(responseItem, response);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Error deserializing mocked error body");
                    httpContext.Items.Add(responseItem, mockedError.Body);
                }

                executedContext.Result = new ObjectResult(response ?? mockedError.Body)
                {
                    StatusCode = (int?)mockedError.HttpStatusCode
                };
                break;
            }
            case MediaTypeNames.Text.Plain or MediaTypeNames.Text.Xml or MediaTypeNames.Text.Html or _:
            {
                httpContext.Items.Add(responseItem, mockedError.Body);
                executedContext.Result = new ContentResult
                {
                    Content = mockedError.Body,
                    StatusCode = (int?)mockedError.HttpStatusCode,
                    ContentType = mockedError.ContentType
                };
                break;
            }
        }

        var updMockQuery = dbContext.Set<MockedError>()
            .Where(e => e.Id == mockedError.Id);

        var updMockRows = mockedError.Count > 1
            ? await updMockQuery.ExecuteUpdateAsync(e => e.SetProperty(p => p.Count, p => p.Count - 1))
            : await updMockQuery.ExecuteDeleteAsync();

        if (updMockRows is not 1)
            _logger.LogCritical("Error mock executed but {AffectedRows} rows affected on upd/del", updMockRows);
    }
}