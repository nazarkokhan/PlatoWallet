namespace PlatipusWallet.Api.StartupSettings.JsonConverters;

// public class JsonCorruptedNumberConverter : JsonConverter<decimal>
// {
//     public override decimal Read(
//         ref Utf8JsonReader reader,
//         Type typeToConvert,
//         JsonSerializerOptions options)
//     {
//         var str = reader.GetString()!;
//
//         str = str.Replace(',', '.');
//
//         var separatorIndex = str.IndexOf('.');
//         
//         for (var i = 0; i < str.Length; i++)
//         {
//             
//         }
//     }
//
//     public override void Write(
//         Utf8JsonWriter writer,
//         decimal value,
//         JsonSerializerOptions options)
//     {
//         writer.WriteStringValue(value ? "1" : "0");
//     }
// }