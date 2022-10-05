using System.Net.Mime;
using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation;
using JorgeSerrano.Json;
using MediatR;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlatipusWallet.Api.Application.Services.GamesApiService;
using PlatipusWallet.Api.Extensions;
using PlatipusWallet.Api.Filters;
using PlatipusWallet.Api.Options;
using PlatipusWallet.Api.StartupSettings.Extensions;
using PlatipusWallet.Api.StartupSettings.JsonConverters;
using PlatipusWallet.Api.StartupSettings.Middlewares;
using PlatipusWallet.Api.StartupSettings.ServicesRegistrations;
using PlatipusWallet.Infrastructure.Persistence;
using Serilog;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(
    (context, configuration) =>
    {
        configuration.EnableSelfLog(context)
            .ReadFrom.Configuration(context.Configuration);
    });

var builderConfiguration = builder.Configuration;
var services = builder.Services;

services
    .AddHttpLogging(
        options =>
        {
            foreach (var allowedHeader in StartupConstants.AllowedHeaders)
                options.RequestHeaders.Add(allowedHeader);
            
            options.LoggingFields = HttpLoggingFields.All;
            options.RequestBodyLogLimit = 1 * 1024 * 1024; //1 MB
            options.RequestBodyLogLimit = 1 * 1024 * 1024; //1 MB
        })
    .AddTransient<VerifySignatureMiddleware>()
    .AddTransient<TestBodyHashingMiddleware>()
    .AddControllers(
        options =>
        {
            options.Filters.Add<ActionResultFilterAttribute>();
            options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
        })
    .AddJsonOptions(
        options =>
        {
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString;
            options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new JsonBoolAsNumberStringConverter());
        })
    .Services
    .Configure<JsonOptions>(
        options =>
        {
            options.SerializerOptions.NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString;
            options.SerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.SerializerOptions.Converters.Add(new JsonBoolAsNumberStringConverter());
        })
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
    .AddHttpClient<IGamesApiClient, GamesApiClient>(options => { options.BaseAddress = new Uri("https://test.platipusgaming.com/psw/"); })
    .AddHttpMessageHandler<RequestSignatureRelegatingHandler>()
    .Services
    .AddStackExchangeRedisCache(r => { r.Configuration = builderConfiguration.GetConnectionString("RedisCache"); });

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpLogging();

app.UseRequestLocalization();

app.UseMiddleware<TestBodyHashingMiddleware>();
app.UseMiddleware<VerifySignatureMiddleware>();

app.MapControllers();

app.Seed();

await app.RunAsync();