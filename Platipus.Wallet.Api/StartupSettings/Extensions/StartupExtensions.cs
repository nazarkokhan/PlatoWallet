using Platipus.Wallet.Api.StartupSettings.Filters.Security.AtlasPlatform;

namespace Platipus.Wallet.Api.StartupSettings.Extensions;

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ControllerSpecificJsonOptions;
using Domain.Entities.Enums;
using Filters.NewFilterStyle;
using Filters.Security;
using Filters.Security.Evenbet;
using Filters.Security.Uranus;
using Filters.Swagger;
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
           .AddSingleton<SoftswissMockedErrorActionFilter>()
           .AddScoped<SoftswissSecurityFilter>()
           .AddSingleton<BetflagMockedErrorActionFilter>()
           .AddScoped<BetflagSecurityFilter>()
           .AddSingleton<BetconstructMockedErrorActionFilter>()
           .AddScoped<BetconstructSecurityFilter>()
           .AddSingleton<OpenboxMockedErrorActionFilter>()
           .AddSingleton<EmaraPlayMockedErrorActionFilter>()
           .AddScoped<EmaraPlaySecurityFilter>()
           .AddSingleton<AtlasMockedErrorActionFilter>()
           .AddScoped<AtlasSecurityFilter>()
           .AddScoped<AtlasExternalSecurityFilter>()
           .AddSingleton<UranusMockedErrorActionFilter>()
           .AddScoped<UranusSecurityFilter>()
           .AddScoped<UranusExternalSecurityFilter>()
           .AddSingleton<EvenbetMockedErrorActionFilter>()
           .AddScoped<EvenbetSecurityFilter>()
           .AddScoped<AnakatechSecurityFilter>()
           .AddSingleton<AnakatechMockedErrorActionFilter>()
           .AddSingleton<NemesisMockedErrorActionFilter>()
           .AddScoped<NemesisSecurityFilter>()
           .AddSingleton<ParimatchMockedErrorActionFilter>()
           .AddScoped<ParimatchSecurityFilter>();
    }

    public static IServiceCollection AddJsonOptionsForProviders(this IMvcBuilder builder)
    {
        builder
           .AddJsonOptions(
                nameof(WalletProvider.Dafabet),
                options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
           .AddJsonOptions(
                nameof(WalletProvider.Openbox),
                options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.Converters.Add(new JsonUnixDateTimeConverter());
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                })
           .AddJsonOptions(
                nameof(WalletProvider.Hub88),
                options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
           .AddJsonOptions(
                nameof(WalletProvider.Softswiss),
                options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
           .AddJsonOptions(
                nameof(WalletProvider.Sw),
                options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = new JsonLowerCaseNamingPolicy();
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.Converters.Add(new JsonBoolAsNumberStringConverter());
                })
           .AddJsonOptions(
                nameof(WalletProvider.SoftBet),
                options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = new JsonLowerCaseNamingPolicy();
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
           .AddJsonOptions(
                nameof(WalletProvider.Betflag),
                options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
           .AddJsonOptions(
                nameof(WalletProvider.Uis),
                options =>
                {
                    // options.JsonSerializerOptions.Converters.Add(new JsonBoolAsNumberStringConverter());
                })
           .AddJsonOptions(
                nameof(WalletProvider.Reevo),
                options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
                    options.JsonSerializerOptions.Converters.Add(new JsonBoolAsNumberStringConverter());
                })
           .AddJsonOptions(
                nameof(WalletProvider.Everymatrix),
                options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                })
           .AddJsonOptions(
                nameof(WalletProvider.BetConstruct),
                _ =>
                {
                })
           .AddJsonOptions(
                nameof(WalletProvider.EmaraPlay),
                options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                })
           .AddJsonOptions(
                nameof(WalletProvider.Atlas),
                options =>
                {
                    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                })
           .AddJsonOptions(
                nameof(WalletProvider.Uranus),
                options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict;
                })
           .AddJsonOptions(
                nameof(WalletProvider.Evenbet),
                options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                })
           .AddJsonOptions(
                nameof(WalletProvider.Anakatech),
                options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                })
           .AddJsonOptions(
                nameof(WalletProvider.Nemesis),
                options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
                    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString
                                                                 | JsonNumberHandling.WriteAsString;
                })
           .AddJsonOptions(
                nameof(WalletProvider.Parimatch),
                options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });

        return builder.Services;
    }

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
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

                options.UseInlineDefinitionsForEnums();
                options.SchemaFilter<RemoveNullPropertiesSchemaFilter>();
            });

        return services;
    }
}