namespace Platipus.Wallet.Api.Extensions;

using System.Text.Json;

public static class JsonDocumentExtensions
{
    public static object? ToConcreteObject(this JsonDocument source)
    {
        return ConvertElementToObject(source.RootElement);

        object? ConvertElementToObject(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                {
                    var dic = element
                       .EnumerateObject()
                       .ToDictionary(k => k.Name, v => ConvertElementToObject(v.Value));
                    dic.Add("$type", "ErrorMock");

                    return dic;
                }

                case JsonValueKind.Array:
                    return element.EnumerateArray()
                       .Select(ConvertElementToObject)
                       .ToArray();

                case JsonValueKind.Number:
                    return element.GetDecimal();

                case JsonValueKind.True or JsonValueKind.False:
                    return element.GetBoolean();

                default:
                    return element.GetString();
            }
        }
    }
}