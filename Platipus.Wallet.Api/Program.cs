using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Humanizer;
using JorgeSerrano.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Platipus.Api.Common;
using Platipus.Serilog;
using Platipus.Wallet.Api;
using Platipus.Wallet.Api.Application.Services.AnakatechGameApi;
using Platipus.Wallet.Api.Application.Services.AtlasGameApi;
using Platipus.Wallet.Api.Application.Services.EmaraPlayGameApi;
using Platipus.Wallet.Api.Application.Services.EvenbetGameApi;
using Platipus.Wallet.Api.Application.Services.EverymatrixGameApi;
using Platipus.Wallet.Api.Application.Services.NemesisGameApi;
using Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.Hub88GamesApi;
using Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.ReevoGamesApi;
using Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.SoftswissGamesApi;
using Platipus.Wallet.Api.Application.Services.PswGameApi;
using Platipus.Wallet.Api.Application.Services.SwGameApi;
using Platipus.Wallet.Api.Application.Services.UisGamesApi;
using Platipus.Wallet.Api.Application.Services.UranusGamesApi;
using Platipus.Wallet.Api.Application.Services.Wallet;
using Platipus.Wallet.Api.Extensions;
using Platipus.Wallet.Api.StartupSettings.Extensions;
using Platipus.Wallet.Api.StartupSettings.Factories;
using Platipus.Wallet.Api.StartupSettings.Filters;
using Platipus.Wallet.Api.StartupSettings.JsonConverters;
using Platipus.Wallet.Api.StartupSettings.Logging;
using Platipus.Wallet.Api.StartupSettings.Middlewares;
using Platipus.Wallet.Api.StartupSettings.Xml;
using Platipus.Wallet.Infrastructure.Persistence;
using Serilog;

try
{
    SelfLogHelper.EnableConsoleAndFile();
    Log.Logger = new LoggerConfiguration()
       .Enrich.FromLogContext()
       .Enrich.WithMachineName()
       .Enrich.WithEnvironmentName()
       .Enrich.WithEnvironmentUserName()
       .Enrich.WithAppVersion()
       .Destructure.ByTransformingWhere<JsonDocument>(t => t == typeof(JsonDocument), v => v.ToConcreteObject()!)
       .WriteTo.File("./logs/static-logger.txt")
       .CreateBootstrapLogger();

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog(
        (context, configuration) => configuration.ReadFrom.Configuration(context.Configuration)
           .Destructure.ByTransformingWhere<JsonDocument>(t => t == typeof(JsonDocument), v => v.ToConcreteObject()!));

    var builderConfiguration = builder.Configuration;
    var services = builder.Services;

    const string gamesApiUrl = "https://test.platipusgaming.com/"; //TODO now it is dynamic from config, remove

    // Middlewares
    services
       .AddSingleton<BufferResponseBodyMiddleware>()
       .AddSingleton<BufferRequestBodyMiddleware>()
       .AddSingleton<LoggingMiddleware>()
       .AddSingleton<ExceptionHandlerMiddleware>();

    services
       .AddScoped<IWalletService, WalletService>()
       .AddControllers(
            options =>
            {
                var xmlFormatter = options.OutputFormatters.OfType<XmlSerializerOutputFormatter>().FirstOrDefault();
                if (xmlFormatter is not null)
                    options.OutputFormatters.Remove(xmlFormatter);

                options.OutputFormatters.Add(new CustomXmlSerializerOutputFormatter());

                // Action
                options.Filters.Add<SaveRequestDataFilterAttribute>(-50000);

                // Result
                options.Filters.Add<ResultToResponseResultFilterAttribute>(1);
            })
       .AddJsonOptions(
            options =>
            {
                options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.WriteAsString
                                                             | JsonNumberHandling.AllowReadingFromString;

                options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new JsonBoolAsNumberStringConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            })
       .ConfigureApiBehaviorOptions(
            options =>
            {
                options.InvalidModelStateResponseFactory =
                    context =>
                    {
                        //TODO suppress and move to ResultToResponseResultFilterAttribute
                        var errors = context.ModelState
                           .Where(x => x.Value?.Errors.Count > 0)
                           .SelectMany(kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage));

                        const ErrorCode code = ErrorCode.ValidationError;
                        var description = string.Join(". ", errors);

                        var errorResponse = new
                        {
                            Code = (int)code,
                            Description = !string.IsNullOrWhiteSpace(description)
                                ? description
                                : code.Humanize()
                        };

                        return new BadRequestObjectResult(errorResponse);
                    };
            })
       .AddJsonOptionsForProviders()
       .AddSecurityAndErrorMockFilters()
       .AddOptions(builderConfiguration)
       .AddEndpointsApiExplorer()
       .AddFluentValidationAutoValidation()
       .AddSwaggerWithConfig()
       .AddMediatR(configuration =>
                   {
                       configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
                       // configuration.AddBehavior( //TODO is ok? Add filter ITransactionRequest
                       //     typeof(IPipelineBehavior<,>),
                       //     typeof(TransactionBehavior<,>),
                       //     ServiceLifetime.Scoped);
                   })
       .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
       .AddLocalization()
       .AddLazyCache()
       .AddDbContext<WalletDbContext>(
            optionsBuilder =>
            {
                optionsBuilder
                   .UseNpgsql(builderConfiguration.GetConnectionString(nameof(WalletDbContext)))
                   .UseSnakeCaseNamingConvention();

                if (builder.Environment.IsDevelopment() || builder.Environment.IsDebug())
                    optionsBuilder.EnableSensitiveDataLogging();
            })
       .AddTransient<SupportedCurrenciesFactory>();

    // GameServer APIs
    services
       .AddTransient<IPswGameApiClient, PswGameApiClient>()
       .AddHttpClient<IPswGameApiClient, PswGameApiClient>(
            options =>
            {
                //TODO need to remove base urls after refactoring old game clients
                options.BaseAddress = new Uri($"{gamesApiUrl}psw/");
            })
       .Services
       .AddTransient<IHub88GamesApiClient, Hub88GamesApiClient>()
       .AddHttpClient<IHub88GamesApiClient, Hub88GamesApiClient>(
            options =>
            {
                options.BaseAddress = new Uri($"{gamesApiUrl}hub88/");
            })
       .Services
       .AddTransient<ISoftswissGamesApiClient, SoftswissGamesApiClient>()
       .AddHttpClient<ISoftswissGamesApiClient, SoftswissGamesApiClient>(
            options =>
            {
                options.BaseAddress = new Uri($"{gamesApiUrl}softswiss/");
            })
       .Services
       .AddTransient<IReevoGameApiClient, ReevoGameApiClient>()
       .AddHttpClient<IReevoGameApiClient, ReevoGameApiClient>(
            options =>
            {
                options.BaseAddress = new Uri($"{gamesApiUrl}reevo/");
            })
       .Services
       .AddTransient<IUisGameApiClient, UisGameApiClient>()
       .AddHttpClient<IUisGameApiClient, UisGameApiClient>()
       .Services
       .AddTransient<IEmaraPlayGameApiClient, EmaraPlayGameApiClient>()
       .AddHttpClient<IEmaraPlayGameApiClient, EmaraPlayGameApiClient>()
       .SetHandlerLifetime(TimeSpan.FromMinutes(5))
       .Services
       .AddTransient<IAtlasGameApiClient, AtlasGameApiClient>()
       .AddHttpClient<IAtlasGameApiClient, AtlasGameApiClient>()
       .Services
       .AddTransient<IUranusGameApiClient, UranusGameApiClient>()
       .AddHttpClient<IUranusGameApiClient, UranusGameApiClient>()
       .Services
       .AddTransient<IEvenbetGameApiClient, EvenbetGameApiClient>()
       .AddHttpClient<IEvenbetGameApiClient, EvenbetGameApiClient>()
       .Services
       .AddTransient<IAnakatechGameApiClient, AnakatechGameApiClient>()
       .AddHttpClient<IAnakatechGameApiClient, AnakatechGameApiClient>()
       .Services
       .AddTransient<INemesisGameApiClient, NemesisGameApiClient>()
       .AddHttpClient<INemesisGameApiClient, NemesisGameApiClient>()
       .Services
       .AddTransient<ISwGameApiClient, SwGameApiClient>()
       .AddHttpClient<ISwGameApiClient, SwGameApiClient>()
       .Services
       .AddTransient<IEverymatrixGameApiClient, EverymatrixGameApiClient>()
       .AddHttpClient<IEverymatrixGameApiClient, EverymatrixGameApiClient>();

    services
       .AddHealthChecks()
       .AddNpgSql(builderConfiguration.GetConnectionString(nameof(WalletDbContext))!, name: nameof(WalletDbContext));

    services.AddHttpContextAccessor();

    var app = builder.Build();

    if (!app.Environment.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<LoggingMiddleware>();

    app.UseRouting();

    app.BufferRequestBody();
    app.BufferResponseBody();

    app.UseMiddleware<ExceptionHandlerMiddleware>();

    app.MapVersion();
    app.MapConfigname();
    app.MapConfig();
    app.MapHealth();
    app.MapVersionTest();

    app.MapControllers();

    await app.SeedAsync();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Startup exception");
}
finally
{
    Log.CloseAndFlush();
}