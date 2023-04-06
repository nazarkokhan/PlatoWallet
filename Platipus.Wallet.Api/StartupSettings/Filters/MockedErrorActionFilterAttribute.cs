namespace Platipus.Wallet.Api.StartupSettings.Filters;

using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Text.Json;
using Api.Extensions;
using Application.Requests.Wallets.BetConstruct.Base;
using Application.Requests.Wallets.Betflag.Base;
using Application.Requests.Wallets.Dafabet.Base;
using Application.Requests.Wallets.Everymatrix.Base;
using Application.Requests.Wallets.Hub88.Base;
using Application.Requests.Wallets.Openbox;
using Application.Requests.Wallets.Openbox.Base;
using Application.Requests.Wallets.Psw.Base;
using Application.Requests.Wallets.Reevo.Base;
using Application.Requests.Wallets.SoftBet.Base;
using Application.Requests.Wallets.Softswiss.Base;
using Application.Requests.Wallets.Sw.Base;
using Application.Requests.Wallets.TODO.EmaraPlay.Base;
using Application.Requests.Wallets.Uis;
using Application.Requests.Wallets.Uis.Base;
using Controllers;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using LazyCache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;

public class MockedErrorActionFilterAttribute : ActionFilterAttribute
{
    [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var services = httpContext.RequestServices;
        var logger = services.GetRequiredService<ILogger<MockedErrorActionFilterAttribute>>();

        logger.LogDebug("Handling request with possible mocked error");

        var executedContext = await next();

        MockedErrorMethod? currentMethod;
        string? usernameOrSession;
        var searchMockBySession = false;

        var requestRoute = context.ActionDescriptor.EndpointMetadata
            .OfType<HttpMethodAttribute>()
            .SingleOrDefault()
            ?.Template;

        var actionArgumentsValues = context.ActionArguments.Values;
        if (context.Controller is WalletOpenboxController)
        {
            var singleRequest = actionArgumentsValues.OfType<OpenboxSingleRequest>().Single();

            if (!httpContext.Items.TryGetValue(HttpContextItems.OpenboxPayloadRequestObj, out var payloadRequestObj)
             || payloadRequestObj is not IOpenboxBaseRequest openboxPayloadObj)
            {
                LogOpenboxMockingFailed(logger, singleRequest);
                return;
            }

            usernameOrSession = openboxPayloadObj.Token;
            searchMockBySession = true;
            currentMethod = singleRequest.Method switch
            {
                OpenboxHelpers.GetPlayerBalance => MockedErrorMethod.Balance,
                OpenboxHelpers.MoneyTransactions => ((OpenboxMoneyTransactionRequest)openboxPayloadObj).OrderType switch
                {
                    3 => MockedErrorMethod.Bet,
                    4 => MockedErrorMethod.Win,
                    _ => null
                },
                OpenboxHelpers.CancelTransaction => MockedErrorMethod.Rollback,
                _ => null
            };
        }
        else if (context.Controller is WalletISoftBetController)
        {
            var singleRequest = actionArgumentsValues.OfType<SoftBetSingleRequest>().Single();

            usernameOrSession = singleRequest.Username;
            currentMethod = singleRequest.Action.Command switch
            {
                "balance" => MockedErrorMethod.Balance,
                "bet" => MockedErrorMethod.Bet,
                "win" => MockedErrorMethod.Win,
                "cancel" => MockedErrorMethod.Rollback,
                _ => null
            };
        }
        else if (context.Controller is WalletReevoController)
        {
            var singleRequest = actionArgumentsValues.OfType<ReevoSingleRequest>().Single();

            usernameOrSession = singleRequest.Username;
            currentMethod = singleRequest.Action switch
            {
                "balance" => MockedErrorMethod.Balance,
                "debit" => MockedErrorMethod.Bet,
                "credit" => MockedErrorMethod.Win,
                "rollback" => MockedErrorMethod.Rollback,
                _ => null
            };
        }
        else if (context.Controller is WalletUisController)
        {
            if (requestRoute is null)
            {
                logger.LogCritical("Request route not found");
                return;
            }

            var baseRequest = context.ActionArguments.Values.OfType<IUisRequest>().Single();
            if (baseRequest is not IUisUserIdRequest request)
                return;

            switch (requestRoute)
            {
                case "get-balance":
                    currentMethod = MockedErrorMethod.Balance;
                    break;
                case "change-balance":
                {
                    var changeBalanceRequest = (UisChangeBalanceRequest)request;

                    currentMethod = changeBalanceRequest.TrnType switch
                    {
                        "BET" => MockedErrorMethod.Bet,
                        "WIN" => MockedErrorMethod.Win,
                        "CANCELBET" => MockedErrorMethod.Rollback,
                        _ => null
                    };
                    break;
                }
                default:
                    return;
            }

            usernameOrSession = request.UserId;
        }
        else
        {
            if (requestRoute is null)
            {
                logger.LogCritical("Request route not found");
                return;
            }

            currentMethod = requestRoute switch
            {
                "balance" or "balance-md5" or "balance-hash" or "user/balance" or "GetBalance" or "GetPlayerInfo"
                    => MockedErrorMethod.Balance,
                "bet" or "bet-win" or "play" or "transaction/bet" or "debit" or "Bet" or "Deposit" => MockedErrorMethod.Bet,
                "win" or "result" or "transaction/win" or "credit" or "Win" or "Withdraw" => MockedErrorMethod.Win,
                "award" or "bonusWin" or "freespin" or "freespins" => MockedErrorMethod.Award,
                "rollback" or "cancel" or "refund" or "transaction/rollback" or "Cancel" or "Rollback"
                    => MockedErrorMethod.Rollback,
                _ => null
            };

            switch (context.Controller)
            {
                case WalletPswController:
                    usernameOrSession = actionArgumentsValues.OfType<IPswBaseRequest>().SingleOrDefault()?.User;
                    break;
                case WalletDafabetController:
                    usernameOrSession = actionArgumentsValues.OfType<IDafabetRequest>().SingleOrDefault()?.PlayerId;
                    break;
                case WalletOpenboxController:
                    usernameOrSession = actionArgumentsValues.OfType<IOpenboxBaseRequest>().SingleOrDefault()?.Token;
                    searchMockBySession = true;
                    break;
                case WalletHub88Controller:
                    usernameOrSession = actionArgumentsValues.OfType<IHub88BaseRequest>().SingleOrDefault()?.SupplierUser;
                    break;
                case WalletSoftswissController:
                    usernameOrSession = actionArgumentsValues.OfType<ISoftswissBaseRequest>().SingleOrDefault()?.UserId;
                    break;
                case WalletSwController:
                    usernameOrSession = actionArgumentsValues.OfType<ISwBaseRequest>().SingleOrDefault()?.Token;
                    searchMockBySession = true;
                    break;
                case WalletBetflagController:
                    usernameOrSession = actionArgumentsValues.OfType<IBetflagRequest>().SingleOrDefault()?.Key;
                    searchMockBySession = true;
                    break;
                case WalletReevoController:
                    usernameOrSession = actionArgumentsValues.OfType<IReevoRequest>().SingleOrDefault()?.Username;
                    break;
                case WalletEverymatrixController:
                    usernameOrSession = actionArgumentsValues.OfType<IEveryMatrixRequest>().SingleOrDefault()?.Token;
                    searchMockBySession = true;
                    break;
                case WalletBetConstructController:
                    usernameOrSession = actionArgumentsValues.OfType<IBetconstructRequest>().SingleOrDefault()?.Token;
                    searchMockBySession = true;
                    break;
                case WalletEmaraPlayController:
                    usernameOrSession = actionArgumentsValues.OfType<IEmaraPlayBaseRequest>().SingleOrDefault()?.Token;
                    searchMockBySession = true;
                    break;
                default:
                    usernameOrSession = null;
                    break;
            }
        }

        if (currentMethod is null)
        {
            logger.LogCritical("ErrorMockMethod not found");
            return;
        }

        if (usernameOrSession is null)
        {
            logger.LogCritical("Can not mock error for request because Username is empty");
            return;
        }

        var cache = httpContext.RequestServices.GetRequiredService<IAppCache>();

        var concurrencyKey = $"em:{usernameOrSession}:{currentMethod.ToString()}";
        var semaphoreSlim = cache.GetOrAdd(concurrencyKey, () => new SemaphoreSlim(1), TimeSpan.FromHours(1));

        try
        {
            await semaphoreSlim.WaitAsync();

            var dbContext = services.GetRequiredService<WalletDbContext>();

            var mockedErrorQuery = dbContext.Set<MockedError>()
                .Where(e => e.Method == currentMethod)
                .Where(
                    e => searchMockBySession
                        ? e.User.Sessions.Any(s => s.Id == usernameOrSession)
                        : e.User.Username == usernameOrSession);

            var mockedError = await mockedErrorQuery
                .OrderBy(e => e.ExecutionOrder)
                .FirstOrDefaultAsync();

            if (mockedError is null)
            {
                logger.LogDebug("Mocked error not found");
                return;
            }

            logger.LogInformation(
                "Executing mocked error {@MockedError}",
                new
                {
                    mockedError.Id,
                    ExecutionConcurrencyKey = concurrencyKey,
                    SessionOrUsername = usernameOrSession,
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
                        context.HttpContext.Items.Add(responseItem, response);
                    }
                    catch (Exception e)
                    {
                        logger.LogWarning(e, "Error deserializing mocked error body");
                        context.HttpContext.Items.Add(responseItem, mockedError.Body);
                    }

                    executedContext.Result = new ObjectResult(response ?? mockedError.Body)
                    {
                        StatusCode = (int?)mockedError.HttpStatusCode
                    };
                    break;
                }
                case MediaTypeNames.Text.Plain or MediaTypeNames.Text.Xml or MediaTypeNames.Text.Html or _:
                {
                    context.HttpContext.Items.Add(responseItem, mockedError.Body);
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
                logger.LogCritical("Error mock executed but {AffectedRows} rows affected on upd/del", updMockRows);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Mocking error failed");
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private static void LogOpenboxMockingFailed(ILogger logger, OpenboxSingleRequest openboxRequest)
        => logger.LogCritical("Failed mocking openbox request. {OpenboxRequest}", openboxRequest);
}