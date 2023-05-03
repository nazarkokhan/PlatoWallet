namespace Platipus.Wallet.Api.StartupSettings.Extensions;

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ControllerSpecificJsonOptions;
using Domain.Entities;
using Domain.Entities.Enums;
using Filters.NewFilterStyle;
using Filters.Security;
using Infrastructure.Persistence;
using JorgeSerrano.Json;
using JsonConverters;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Options;
using Swashbuckle.AspNetCore.SwaggerGen;

public static class StartupExtensions
{
    public static IServiceCollection AddSecurityAndErrorMockFilters(this IServiceCollection services)
    {
        return services
            .AddSingleton<BetflagMockedErrorActionFilter>()
            .AddScoped<BetflagSecurityFilter>()
            .AddSingleton<BetconstructMockedErrorActionFilter>()
            .AddScoped<BetconstructSecurityFilter>()
            .AddScoped<OpenboxMockedErrorActionFilter>(); //scoped because it needs httpContext
    }

    public static IServiceCollection AddJsonOptionsForProviders(this IMvcBuilder builder)
    {
        builder
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
            .AddJsonOptions(
                nameof(CasinoProvider.Uis),
                options =>
                {
                    // options.JsonSerializerOptions.Converters.Add(new JsonBoolAsNumberStringConverter());
                })
            .AddJsonOptions(
                nameof(CasinoProvider.Reevo),
                options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
                    options.JsonSerializerOptions.Converters.Add(new JsonBoolAsNumberStringConverter());
                })
            .AddJsonOptions(
                nameof(CasinoProvider.Everymatrix),
                options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                })
            .AddJsonOptions(
                nameof(CasinoProvider.BetConstruct),
                options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });

        return builder.Services;
    }

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<SupportedCurrenciesOptions>()
            .Configure<WalletDbContext>(
                (options, context) =>
                {
                    var currencies = context.Set<Currency>()
                        .Select(c => c.Id)
                        .ToList();

                    options.Items = new HashSet<string>(currencies);
                })
            .Services
            .AddOptions<SoftswissCurrenciesOptions>()
            .Configure(
                options =>
                {
                    var fullJson = File.ReadAllText("StaticFiles/softswiss_currency_mappers.json");
                    var jsonNode = JsonNode.Parse(fullJson)!;

                    var optionsValue = jsonNode.AsObject()
                        .ToDictionary(
                            x => x.Value!["iso_code"]!.GetValue<string>(),
                            x => x.Value!["subunit_to_unit"]!.GetValue<long>());

                    options.CountryIndexes = optionsValue;
                });

        return services;
    }

    public static IServiceCollection AddSwaggerWithConfig(this IServiceCollection services)
    {
        services.AddSwaggerGen(
            options =>
            {
                options.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "JWTToken_Auth_API",
                        Version = "v1"
                    });
                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description
                            = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                    });
                options.AddSecurityRequirement(
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

                options.MapType<TimeSpan>(
                    () => new OpenApiSchema
                    {
                        Type = "string",
                        Example = new OpenApiString(TimeSpan.Zero.ToString())
                    });
                options.SupportNonNullableReferenceTypes();
                options.MapType<DateOnly>(
                    () => new OpenApiSchema
                    {
                        Type = "string",
                        Format = "date"
                    });
                options.MapType<TimeOnly>(
                    () => new OpenApiSchema
                    {
                        Type = "string",
                        Format = "time",
                        Example = OpenApiAnyFactory.CreateFromJson($"\"{DateTime.UtcNow:HH:mm}\"")
                    });
            });

        return services;
    }
}