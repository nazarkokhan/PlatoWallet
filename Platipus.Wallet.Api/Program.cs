using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation;
using Horizon.XmlRpc.AspNetCore.Extensions;
using JorgeSerrano.Json;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Platipus.Api.Common;
using Platipus.Serilog;
using Platipus.Wallet.Api.Application.Services.GamesGlobalGamesApi;
using Platipus.Wallet.Api.Application.Services.Hub88GamesApi;
using Platipus.Wallet.Api.Application.Services.PswGamesApi;
using Platipus.Wallet.Api.Application.Services.ReevoGamesApi;
using Platipus.Wallet.Api.Application.Services.SoftswissGamesApi;
using Platipus.Wallet.Api.Application.Services.UisGamesApi;
using Platipus.Wallet.Api.Application.Services.Wallet;
using Platipus.Wallet.Api.Controllers.GamesGlobal;
using Platipus.Wallet.Api.Extensions;
using Platipus.Wallet.Api.Obsolete;
using Platipus.Wallet.Api.StartupSettings.Extensions;
using Platipus.Wallet.Api.StartupSettings.Filters;
using Platipus.Wallet.Api.StartupSettings.JsonConverters;
using Platipus.Wallet.Api.StartupSettings.Middlewares;
using Platipus.Wallet.Api.StartupSettings.ServicesRegistrations;
using Platipus.Wallet.Api.StartupSettings.Xml;
using Platipus.Wallet.Infrastructure.Persistence;
using Serilog;
using Serilog.Sinks.Elasticsearch;

try
{
    const string appVersion = "AppVersion";
    var assemblyVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString()!;

    Log.Logger = new LoggerConfiguration()
        .Enrich.WithMachineName()
        .Enrich.WithProperty(appVersion, assemblyVersion, true)
        .WriteTo.Elasticsearch(
            nodeUris: "http://elastic.aws.intra:9200;",
            indexFormat: "platipus-wallet-api",
            connectionGlobalHeaders: "Authorization=Basic cGxhdGlwdXNfZWxhc3RpYzpUaGFpcmFoUGgydXNob28=",
            autoRegisterTemplateVersion: AutoRegisterTemplateVersion.ESv7,
            batchAction: ElasticOpType.Create,
            typeName: null,
            customFormatter: new TargetedElasticsearchJsonFormatter())
        .CreateBootstrapLogger();

    Log.Warning("Starting app");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog(
        (context, configuration) =>
        {
            configuration.EnableSelfLog(context)
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.WithProperty(appVersion, assemblyVersion, true);
        });

    Log.Warning("Reconfigured boostrap logger");

    var builderConfiguration = builder.Configuration;
    var services = builder.Services;

    services.Configure<KestrelServerOptions>(
        options =>
        {
            options.AllowSynchronousIO = true;
        });

    const string gamesApiUrl = "https://test.platipusgaming.com/"; //TODO now it is dynamyc from config
    services
        .AddScoped<IWalletService, WalletService>()
        .AddTransient<ExceptionHandlerMiddleware>()
        .AddTransient<BufferResponseBodyMiddleware>()
        .AddTransient<GamesGlobalMiddleware>()
        .AddTransient<GamesGlobalAuthMiddleware>()
        .AddControllers(
            options =>
            {
                var xmlFormatter = options.OutputFormatters.OfType<XmlSerializerOutputFormatter>().FirstOrDefault();
                if (xmlFormatter != null)
                {
                    options.OutputFormatters.Remove(xmlFormatter);
                }

                options.OutputFormatters.Add(new CustomXmlSerializerOutputFormatter());

                options.Filters.Add<SaveRequestActionFilterAttribute>(1);

                options.Filters.Add<ActionResultFilterAttribute>(1);
                options.Filters.Add<LoggingResultFilterAttribute>(2);
            })
        .AddJsonOptions(
            options =>
            {
                options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.WriteAsString
                                                             | JsonNumberHandling.AllowReadingFromString;
                options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new JsonBoolAsNumberStringConverter());
            })
        .AddJsonOptionsForProviders()
        .AddOptions(builderConfiguration)
        .AddEndpointsApiExplorer()
        .AddSwaggerWithConfig()
        .AddMediatR(Assembly.GetExecutingAssembly())
        .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
        .AddAllBehaviors()
        .AddLocalization()
        .AddLazyCache()
        .AddDbContext<WalletDbContext>(
            (provider, optionsBuilder) =>
            {
                optionsBuilder
                    .UseNpgsql(builderConfiguration.GetConnectionString(nameof(WalletDbContext)))
                    .UseSnakeCaseNamingConvention();

                if (builder.Environment.IsDevelopment() || builder.Environment.IsDebug())
                {
                    optionsBuilder.UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>());
                    optionsBuilder.EnableSensitiveDataLogging();
                }
            })
        .AddSingleton<IPswAndBetflagGameApiClient, PswAndBetflagGameApiClient>()
        .AddHttpClient<IPswAndBetflagGameApiClient, PswAndBetflagGameApiClient>(
            options =>
            {
                options.BaseAddress = new Uri($"{gamesApiUrl}psw/");
            })
        .Services
        .AddSingleton<IHub88GamesApiClient, Hub88GamesApiClient>()
        .AddHttpClient<IHub88GamesApiClient, Hub88GamesApiClient>(
            options =>
            {
                options.BaseAddress = new Uri($"{gamesApiUrl}hub88/");
            })
        .Services
        .AddSingleton<ISoftswissGamesApiClient, SoftswissGamesApiClient>()
        .AddHttpClient<ISoftswissGamesApiClient, SoftswissGamesApiClient>(
            options =>
            {
                options.BaseAddress = new Uri($"{gamesApiUrl}softswiss/");
            })
        .Services
        .AddSingleton<IGamesGlobalGamesApiClient, GamesGlobalGamesApiClient>()
        .AddHttpClient<IGamesGlobalGamesApiClient, GamesGlobalGamesApiClient>(
            options =>
            {
                options.BaseAddress = new Uri($"{gamesApiUrl}gameglobal/");
            })
        .Services
        .AddSingleton<IReevoGameApiClient, ReevoGameApiClient>()
        .AddHttpClient<IReevoGameApiClient, ReevoGameApiClient>(
            options =>
            {
                options.BaseAddress = new Uri($"{gamesApiUrl}reevo/");
            })
        .Services
        .AddSingleton<IUisGameApiClient, UisGameApiClient>()
        .AddHttpClient<IUisGameApiClient, UisGameApiClient>();

    services.AddHealthChecks();

    services.AddXmlRpc();

    var app = builder.Build();

    app.UseExceptionHandler(
        exceptionAppBuilder =>
        {
            exceptionAppBuilder.UseMiddleware<ExceptionHandlerMiddleware>();
        });

    if (!app.Environment.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.EnableBufferingAndSaveRawRequest();

    app.UseRequestLocalization();

    app.UseMiddleware<BufferResponseBodyMiddleware>();
    app.UseMiddleware<GamesGlobalMiddleware>();
    app.UseXmlRpc(
        configure =>
        {
            configure.MapService<WalletGamesGlobalService>("wallet/games-global/gaming");
            configure.MapService<WalletGamesGlobalAdminService>("wallet/games-global/admin");
        });

    app.MapVersion();
    app.MapConfigname();
    app.MapHealthz();

    app.MapControllers();

    await app.SeedAsync();

    await app.RunAsync();
}

catch (Exception ex)
{
    Log.Fatal(ex, "FAILED!!!! on startup");
}
finally
{
    Log.Fatal("Flushing before closing app");
    Log.CloseAndFlush();
}