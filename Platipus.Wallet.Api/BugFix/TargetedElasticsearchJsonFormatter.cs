// namespace Platipus.Wallet.Api.BugFix;
//
// using System.Reflection;
// using Serilog.Events;
// using Serilog.Formatting.Elasticsearch;
//
// public class TargetedElasticsearchJsonFormatter : ElasticsearchJsonFormatter
// {
//     private readonly bool _inlineFields;
//     private readonly string _target;
//
//     public TargetedElasticsearchJsonFormatter(
//         string? target = null,
//         bool omitEnclosingObject = false,
//         string? closingDelimiter = null,
//         bool renderMessage = true,
//         IFormatProvider? formatProvider = null,
//         ISerializer? serializer = null,
//         bool inlineFields = false,
//         bool renderMessageTemplate = true,
//         bool formatStackTraceAsArray = false)
//         : base(
//             omitEnclosingObject,
//             closingDelimiter,
//             renderMessage,
//             formatProvider,
//             serializer,
//             inlineFields,
//             renderMessageTemplate,
//             formatStackTraceAsArray)
//     {
//         _target = target ?? Assembly.GetEntryAssembly()?.GetName().Name?.Replace('.', '-').ToLower() ?? "fields";
//         _inlineFields = inlineFields;
//     }
//
//     protected override void WriteProperties(IReadOnlyDictionary<string, LogEventPropertyValue> properties, TextWriter output)
//     {
//         if (!_inlineFields)
//             output.Write($",\"{_target}\":{{");
//         else
//             output.Write(",");
//         WritePropertiesValues(properties, output);
//         if (_inlineFields)
//             return;
//         output.Write("}");
//     }
// }