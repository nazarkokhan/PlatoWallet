using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using FluentValidation;
using Horizon.XmlRpc.AspNetCore.Extensions;
using JorgeSerrano.Json;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Platipus.Serilog;
using Platipus.Wallet.Api;
using Platipus.Wallet.Api.Application.Services.GamesApi;
using Platipus.Wallet.Api.Application.Services.GamesGlobalGamesApi;
using Platipus.Wallet.Api.Application.Services.Hub88GamesApi;
using Platipus.Wallet.Api.Application.Services.SoftswissGamesApi;
using Platipus.Wallet.Api.Application.Services.Wallet;
using Platipus.Wallet.Api.Controllers.GamesGlobal;
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
                .Enrich.WithProperty("AppVersion", App.Version, true);
        });

    Log.Warning("Reconfigured boostrap logger");

    var builderConfiguration = builder.Configuration;
    var services = builder.Services;

    services.Configure<KestrelServerOptions>(
        options =>
        {
            options.AllowSynchronousIO = true;
        });

    const string gamesApiUrl = "https://test.platipusgaming.com/"; //TODO to config
    services
        .AddScoped<IWalletService, WalletService>()
        .AddTransient<ExceptionHandlerMiddleware>()
        .AddTransient<GamesGlobalMiddleware>()
        .AddTransient<GamesGlobalAuthMiddleware>()
        .AddControllers(
            options =>
            {
                // var xmlWriterSettings = new XmlWriterSettings
                // {
                //     OmitXmlDeclaration = true,
                //     NewLineHandling =
                // };
                // options.InputFormatters.Add(new XmlSerializerInputFormatter(options));
                // options.OutputFormatters.Add(new XmlSerializerOutputFormatter(xmlWriterSettings));

                options.Filters.Add<SaveRequestActionFilterAttribute>(1);

                options.Filters.Add<ActionResultFilterAttribute>(1);
                options.Filters.Add<LoggingResultFilterAttribute>(2);
            })
        .AddXmlSerializerFormatters()
        .AddJsonOptions(
            options =>
            {
                options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.WriteAsString
                                                             | JsonNumberHandling.AllowReadingFromString;
                options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new JsonBoolAsNumberStringConverter());
            })
        .AddJsonOptions(
            nameof(CasinoProvider.Dafabet),
            options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
        .AddJsonOptions(
            nameof(CasinoProvider.Openbox),
            options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new JsonUnixDateTimeConverter());
            })
        .AddJsonOptions(
            nameof(CasinoProvider.Hub88),
            options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
        .AddJsonOptions(
            nameof(CasinoProvider.Softswiss),
            options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
        .AddJsonOptions(
            nameof(CasinoProvider.Sw),
            options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = new JsonLowerCaseNamingPolicy();
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new JsonBoolAsNumberStringConverter());
            })
        .AddJsonOptions(
            nameof(CasinoProvider.SoftBet),
            options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = new JsonLowerCaseNamingPolicy();
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
        .AddJsonOptions(
            nameof(CasinoProvider.Betflag),
            options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })

        // .AddJsonOptions(
        //     nameof(CasinoProvider.Everymatrix),
        //     options =>
        //     {
        //         options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.WriteAsString
        //                                                      | JsonNumberHandling.AllowReadingFromString;
        //         options.JsonSerializerOptions.PropertyNamingPolicy = null;
        //         options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        //     })
        // .AddJsonOptions(
        //     nameof(CasinoProvider.PariMatch),
        //     options =>
        //     {
        //         options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        //         options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        //     })
        // .AddJsonOptions(
        //     nameof(CasinoProvider.EmaraPlay),
        //     options =>
        //     {
        //         options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        //         options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        //     })
        .Services
        .Configure<SupportedCurrenciesOptions>(builderConfiguration.GetSection(nameof(SupportedCurrenciesOptions)).Bind)
        .Configure<SupportedCountriesOptions>(
            options => builderConfiguration.GetSection(nameof(SupportedCountriesOptions)).Bind(options))
        .Configure(
            () =>
            {
                var fullJson = File.ReadAllText("StaticFiles/softswiss_currency_mappers.json");
                var jsonNode = JsonNode.Parse(fullJson)!;

                var optionsValue = jsonNode.AsObject()
                    .ToDictionary(
                        x => x.Value!["iso_code"]!.GetValue<string>(),
                        x => x.Value!["subunit_to_unit"]!.GetValue<long>());

                return new SoftswissCurrenciesOptions { CountryIndexes = optionsValue };
            })
        .AddEndpointsApiExplorer()
        .AddSwaggerGen(
            c =>
            {
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "JWTToken_Auth_API",
                        Version = "v1"
                    });
                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme()
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description
                            = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                    });
                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] { }
                        }
                    });
            })
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
        .AddHttpClient<IGamesApiClient, GamesApiClient>(
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
        .AddHttpClient(
            "Nazar",
            options =>
            {
                options.BaseAddress = new Uri($"{gamesApiUrl}gameglobal/");
            })
        .Services
        .AddStackExchangeRedisCache(
            r =>
            {
                r.Configuration = builderConfiguration.GetConnectionString("RedisCache");
            });

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

    app.UseMiddleware<GamesGlobalMiddleware>();
    app.UseXmlRpc(
        configure =>
        {
            configure.MapService<WalletGamesGlobalService>("wallet/games-global/gaming");
            configure.MapService<WalletGamesGlobalAdminService>("wallet/games-global/admin");
        });

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

namespace Platipus.Wallet.Api
{
    public static class App
    {
        public const string Version = "22.0";
    }
}