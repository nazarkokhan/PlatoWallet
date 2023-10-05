namespace Platipus.Wallet.Api.Extensions;

using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

public static class ObjectToDictionaryConverter
{
    public static Dictionary<string, string?> ConvertToDictionary(object obj)
    {
        var result = new Dictionary<string, string?>();

        var properties = TypeDescriptor.GetProperties(obj);
        foreach (PropertyDescriptor property in properties)
        {
            var key = property.Name;
            var bindPropertyAttribute = property.Attributes.OfType<BindPropertyAttribute>().FirstOrDefault();
            key = bindPropertyAttribute?.Name ?? key;

            var valueObj = property.GetValue(obj);
            string? value;
            if (valueObj is bool booleanValue)
            {
                value = booleanValue.ToString().ToLowerInvariant();
            }
            else
            {
                value = valueObj?.ToString();
            }
        
            result.Add(key, value);
        }

        return result;
    }

}