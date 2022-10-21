using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation;
using JorgeSerrano.Json;
using MediatR;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using PlatipusWallet.Api.Application.Services.DatabetGamesApi;
using PlatipusWallet.Api.Application.Services.GamesApi;
using PlatipusWallet.Api.Extensions;
using PlatipusWallet.Api.Filters;
using PlatipusWallet.Api.Options;
using PlatipusWallet.Api.StartupSettings;
using PlatipusWallet.Api.StartupSettings.Extensions;
using PlatipusWallet.Api.StartupSettings.JsonConverters;
using PlatipusWallet.Api.StartupSettings.Middlewares;
using PlatipusWallet.Api.StartupSettings.ServicesRegistrations;
using PlatipusWallet.Domain.Entities.Enums;
using PlatipusWallet.Infrastructure.Persistence;
using Serilog;

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
    // .AddHttpLogging(
    //     options =>
    //     {
    //         foreach (var allowedHeader in StartupConstants.AllowedHeaders)
    //             options.RequestHeaders.Add(allowedHeader);
    //
    //         options.LoggingFields = HttpLoggingFields.All;
    //         options.RequestBodyLogLimit = 1 * 1024 * 1024; //1 MB
    //         options.RequestBodyLogLimit = 1 * 1024 * 1024; //1 MB
    //     })
    .AddTransient<VerifySignatureMiddleware>()
    .AddTransient<TestBodyHashingMiddleware>()
    .AddTransient<ExceptionHandlerMiddleware>()
    .AddTransient<LoggingMiddleware>()
    .AddControllers(
        options =>
        {
            options.Filters.Add<ActionResultFilterAttribute>(1);
            // options.Filters.Add<LoggingFilterAttribute>(2);
        })
    .AddJsonOptions(
        options =>
        {
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString;
            options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new JsonBoolAsNumberStringConverter());
        })
    .AddJsonOptions(nameof(CasinoProvider.Dafabet), options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); })
    // .AddXmlSerializerFormatters()
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
app.UseMiddleware<LoggingMiddleware>();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.EnableBuffering();

app.UseRequestLocalization();

app.UseMiddleware<TestBodyHashingMiddleware>();
app.UseMiddleware<VerifySignatureMiddleware>();

app.MapControllers();

app.Seed();

await app.RunAsync();