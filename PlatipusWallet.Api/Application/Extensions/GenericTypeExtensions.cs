namespace PlatipusWallet.Api.Application.Extensions;

using System;
using System.Linq;

public static class GenericTypeExtensions
{
    public static string GetTypeName(this object @object)
    {
        return @object.GetType().GetGenericTypeName();
    }

    private static string GetGenericTypeName(this Type type)
    {
        string typeName;

        if (type.IsGenericType)
        {
            var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name));
            typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
        }
        else
        {
            typeName = type.Name;
        }

        return typeName;
    }
}