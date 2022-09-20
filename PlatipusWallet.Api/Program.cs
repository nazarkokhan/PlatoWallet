using System.Net.Mime;
using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation;
using JorgeSerrano.Json;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlatipusWallet.Api.Extensions;
using PlatipusWallet.Api.Filters;
using PlatipusWallet.Api.Middlewares;
using PlatipusWallet.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

var services = builder.Services;

services
    .AddControllers(
        options =>
        {
            options.Filters.Add<ActionResultFilterAttribute>();
            options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
        })
    .AddJsonOptions(
        options =>
        {
            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.WriteAsString;
            options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        })
    .Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddMediatR(Assembly.GetExecutingAssembly())
    .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
    .AddDbContext<WalletDbContext>(
        (provider, optionsBuilder) =>
        {
            optionsBuilder
                .UseNpgsql(builder.Configuration.GetConnectionString(nameof(WalletDbContext)));

            if (builder.Environment.IsDevelopment())
            {
                optionsBuilder.UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>());
                optionsBuilder.EnableSensitiveDataLogging();
            }
        });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<VerifySignatureMiddleware>();

app.MapControllers();

app.Seed();

app.Run();