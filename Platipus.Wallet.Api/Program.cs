using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Horizon.XmlRpc.AspNetCore.Extensions;
using Humanizer;
using JorgeSerrano.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Platipus.Api.Common;
using Platipus.Serilog;
using Platipus.Wallet.Api.Application.Services.AtlasGamesApi;
using Platipus.Wallet.Api.Application.Services.EmaraPlayGamesApi;
using Platipus.Wallet.Api.Application.Services.GamesGlobalGamesApi;
using Platipus.Wallet.Api.Application.Services.Hub88GamesApi;
using Platipus.Wallet.Api.Application.Services.PswGamesApi;
using Platipus.Wallet.Api.Application.Services.ReevoGamesApi;
using Platipus.Wallet.Api.Application.Services.SoftswissGamesApi;
using Platipus.Wallet.Api.Application.Services.UisGamesApi;
using Platipus.Wallet.Api.Application.Services.Wallet;
using Platipus.Wallet.Api.Extensions;
using Platipus.Wallet.Api.Obsolete;
using Platipus.Wallet.Api.StartupSettings.Extensions;
using Platipus.Wallet.Api.StartupSettings.Factories;
using Platipus.Wallet.Api.StartupSettings.Filters;
using Platipus.Wallet.Api.StartupSettings.JsonConverters;
using Platipus.Wallet.Api.StartupSettings.Logging;
using Platipus.Wallet.Api.StartupSettings.Middlewares;
using Platipus.Wallet.Api.StartupSettings.ServicesRegistrations;
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
       .WriteTo.File("./logs/static-logger.txt")
       .CreateBootstrapLogger();

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

    var builderConfiguration = builder.Configuration;
    var services = builder.Services;

    const string gamesApiUrl = "https://test.platipusgaming.com/"; //TODO now it is dynamic from config, remove
    services
       .AddScoped<IWalletService, WalletService>()
       .AddTransient<ExceptionHandlerMiddleware>()
       .AddTransient<BufferResponseBodyMiddleware>()
       .AddControllers(
            options =>
            {
                var xmlFormatter = options.OutputFormatters.OfType<XmlSerializerOutputFormatter>().FirstOrDefault();
                if (xmlFormatter is not null)
                    options.OutputFormatters.Remove(xmlFormatter);

                options.OutputFormatters.Add(new CustomXmlSerializerOutputFormatter());

                // Action
                options.Filters.Add<SaveRequestActionFilterAttribute>(1);

                // Result
                options.Filters.Add<ResultToResponseResultFilterAttribute>(1);
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
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            })
       .ConfigureApiBehaviorOptions(
            options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                                                           {
                                                               //TODO suppress and move to ResultToResponseResultFilterAttribute
                                                               var errors = context.ModelState
                                                                  .Where(x => x.Value?.Errors.Count > 0)
                                                                  .SelectMany(
                                                                       kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage));

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
       .AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(Program).Assembly))
       .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
       .AddAllBehaviors()
       .AddLocalization()
       .AddLazyCache()
       .AddDbContext<WalletDbContext>(
            (optionsBuilder) =>
            {
                optionsBuilder
                   .UseNpgsql(builderConfiguration.GetConnectionString(nameof(WalletDbContext)))
                   .UseSnakeCaseNamingConvention();

                if (builder.Environment.IsDevelopment() || builder.Environment.IsDebug())
                    optionsBuilder.EnableSensitiveDataLogging();
            })
       .AddTransient<SupportedCurrenciesFactory>()
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
       .AddHttpClient<IUisGameApiClient, UisGameApiClient>()
       .Services
       .AddSingleton<IEmaraPlayGameApiClient, EmaraPlayGameApiClient>()
       .AddHttpClient<IEmaraPlayGameApiClient, EmaraPlayGameApiClient>()
       .SetHandlerLifetime(TimeSpan.FromMinutes(5))
       .Services
       .AddSingleton<IAtlasGameApiClient, AtlasGameApiClient>()
       .AddHttpClient<IAtlasGameApiClient, AtlasGameApiClient>();

    services
       .AddHealthChecks()
       .AddNpgSql(builderConfiguration.GetConnectionString(nameof(WalletDbContext))!, name: nameof(WalletDbContext));

    services.AddXmlRpc();
    services.AddHttpContextAccessor();
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

    var assemblyName = Assembly.GetEntryAssembly()?.FullName!;
    app.MapGet(
        string.Empty,
        async (HttpContext context, IWebHostEnvironment environment) =>
        {
            if (!environment.IsProduction())
                context.Response.Redirect("/swagger/index.html");
            else
                await context.Response.WriteAsync(assemblyName);
        });

    app.MapVersion();
    app.MapConfigname();
    app.MapConfig();
    app.MapHealth("healthz"); // TODO temporary, ask devops to remove z

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