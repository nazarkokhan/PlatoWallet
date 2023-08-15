namespace Platipus.Wallet.Api.StartupSettings.Filters.Swagger;

using System.Reflection;
using Application.Requests.External;
using Attributes.Swagger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public sealed class RemoveNullPropertiesSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type != typeof(LogInRequest) || !HasCustomAttribute())
            return;

        var properties = schema.Properties.Where(p => p.Value.Nullable).ToList();
        foreach (var prop in properties)
        {
            schema.Properties.Remove(prop.Key);
        }
    }

    private static bool HasCustomAttribute()
    {
        var assembly = Assembly.GetExecutingAssembly();
        return (from type in assembly.GetTypes()
                where type.IsSubclassOf(typeof(ControllerBase))
                from method in type.GetMethods()
                let parameters = method.GetParameters()
                where parameters.Any(p => p.ParameterType == typeof(LogInRequest))
                   && method.GetCustomAttribute<ApplyRemoveNullPropertiesAttribute>() is not null
                select method).Any();
    }
}