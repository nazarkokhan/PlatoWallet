using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation;
using JorgeSerrano.Json;
using Microsoft.EntityFrameworkCore;
using Platipus.Serilog;
using Platipus.Wallet.Api.Application.Services.DatabetGamesApi;
using Platipus.Wallet.Api.Application.Services.GamesApi;
using Platipus.Wallet.Api.Extensions;
using Platipus.Wallet.Api.StartupSettings.ControllerSpecificJsonOptions;
using Platipus.Wallet.Api.StartupSettings.Filters;
using Platipus.Wallet.Api.StartupSettings.JsonConverters;
using Platipus.Wallet.Api.StartupSettings.Middlewares;
using Platipus.Wallet.Api.StartupSettings.Options;
using Platipus.Wallet.Api.StartupSettings.ServicesRegistrations;
using Platipus.Wallet.Domain.Entities.Enums;
using Platipus.Wallet.Infrastructure.Persistence;
using Serilog;
using Serilog.Sinks.Elasticsearch;

try
{
    Log.Logger = new LoggerConfiguration()
        .Enrich.WithMachineName()
        .Enrich.WithProperty("AppVersion", App.Version, true)
        .WriteTo.Elasticsearch(
            nodeUris: "http://10.0.3.46:9200;",
            indexFormat: "platipus-wallet",
            connectionGlobalHeaders: "Authorization=Basic cGxhdGlwdXNfZWxhc3RpYzpUaGFpcmFoUGgydXNob28=",
            autoRegisterTemplateVersion: AutoRegisterTemplateVersion.ESv7,
            batchAction: ElasticOpType.Create,
            typeName: null)
        .CreateBootstrapLogger();

    Log.Warning("Starting app");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog(
        (context, configuration) =>
        {
            configuration.EnableSelfLog(context)
                .ReadFrom.Configuration(context.Configuration);
        });

    var builderConfiguration = builder.Configuration;
    var services = builder.Services;

    const string gamesApiUrl = "https://test.platipusgaming.com/"; //TODO to config
    services
        .AddTransient<ExceptionHandlerMiddleware>()
        .AddControllers(
            options =>
            {
                options.Filters.Add<SaveRequestActionFilterAttribute>(1);

                options.Filters.Add<ActionResultFilterAttribute>(1);
                options.Filters.Add<LoggingResultFilterAttribute>(2);
            })
        .AddJsonOptions(
            options =>
            {
                options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString;
                options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new JsonBoolAsNumberStringConverter());
            })
        .AddJsonOptions(
            nameof(CasinoProvider.Dafabet),
            options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); })
        .AddJsonOptions(
            nameof(CasinoProvider.Openbox),
            options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new JsonUnixDateTimeConverter());
            })
        .Services
        .Configure<SupportedCurrenciesOptions>(builderConfiguration.GetSection(nameof(SupportedCurrenciesOptions)).Bind)
        .Configure<SupportedCountriesOptions>(builderConfiguration.GetSection(nameof(SupportedCountriesOptions)).Bind)
        .AddEndpointsApiExplorer()
        .AddSwaggerGen()
        .AddMediatR(Assembly.GetExecutingAssembly())
        .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
        .AddAllBehaviors()
        .AddLocalization()
        .AddLazyCache()
        .AddDbContext<WalletDbContext>(
            (provider, optionsBuilder) =>
            {
                optionsBuilder
                    .UseNpgsql(builderConfiguration.GetConnectionString(nameof(WalletDbContext)));

                if (builder.Environment.IsDevelopment())
                {
                    optionsBuilder.UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>());
                    optionsBuilder.EnableSensitiveDataLogging();
                }
            })
        .AddSingleton<IGamesApiClient, GamesApiClient>()
        .AddTransient<RequestSignatureRelegatingHandler>()
        .AddHttpClient<IGamesApiClient, GamesApiClient>(options => { options.BaseAddress = new Uri($"{gamesApiUrl}psw/"); })
        .AddHttpMessageHandler<RequestSignatureRelegatingHandler>()
        .Services
        .AddSingleton<IDatabetGamesApiClient, DatabetGamesApiClient>()
        .AddHttpClient<IDatabetGamesApiClient, DatabetGamesApiClient>(options => { options.BaseAddress = new Uri($"{gamesApiUrl}dafabet/"); })
        .Services
        .AddStackExchangeRedisCache(r => { r.Configuration = builderConfiguration.GetConnectionString("RedisCache"); });


    var app = builder.Build();

    app.UseExceptionHandler(exceptionAppBuilder => { exceptionAppBuilder.UseMiddleware<ExceptionHandlerMiddleware>(); });

    if (!app.Environment.IsProduction())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.EnableBufferingAndSaveRawRequest();

    app.UseRequestLocalization();

    app.MapControllers();

    app.Seed();

    await app.RunAsync();
}

catch (Exception ex)
{
    Log.Fatal(ex, "FAILED!!!! on startup");
}
finally
{
    Log.CloseAndFlush();
}

public static class App
{
    public const string Version = "5.0";
}