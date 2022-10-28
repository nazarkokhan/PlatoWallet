namespace PlatipusWallet.Api.StartupSettings.Extensions;

using System.Reflection;
using Serilog;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Sinks.Elasticsearch;

public static class SerilogElasticsearchConfigurationExtensions
{
    private const string LogEventSinksFieldName = "_logEventSinks";
    private const string StateFieldName = "_state";
    private const string OptionsPropertyName = "Options";

    public static ElasticsearchSinkOptions? GetElasticsearchSinkOptions(this LoggerConfiguration configuration)
    {
        var logEventSinksField = configuration.GetType().GetRuntimeFields().FirstOrDefault(x => x.Name == LogEventSinksFieldName);
        var logEventSinksValue = logEventSinksField?.GetValue(configuration);

        if (logEventSinksValue is not List<ILogEventSink> sinksList)
        {
            SelfLog.WriteLine($"Could not get {LogEventSinksFieldName} field from logger configuration");
            return null;
        }

        var elasticsearchSinkValue = sinksList.FirstOrDefault(s => s.GetType() == typeof(ElasticsearchSink));

        if (elasticsearchSinkValue is not ElasticsearchSink elasticsearchSink)
        {
            SelfLog.WriteLine($"Could not get {nameof(ElasticsearchSink)} sink from {LogEventSinksFieldName} configuration field");
            return null;
        }

        var stateField = elasticsearchSink.GetType().GetRuntimeFields().FirstOrDefault(s => s.Name == StateFieldName);
        var stateValue = stateField?.GetValue(elasticsearchSink);

        if (stateValue is null)
        {
            SelfLog.WriteLine($"Could not get {StateFieldName} field from {nameof(ElasticsearchSink)}");
            return null;
        }

        var optionsProperty = stateValue.GetType().GetRuntimeProperties().FirstOrDefault(s => s.Name == OptionsPropertyName);
        var optionsValue = optionsProperty?.GetValue(stateValue);

        if (optionsValue is not ElasticsearchSinkOptions options)
        {
            SelfLog.WriteLine($"{nameof(ElasticsearchSinkOptions)} not found in {StateFieldName} field value");
            return null;
        }

        return options;
    }

    public static LoggerConfiguration ConfigureElasticsearchSinkOptions(
        this LoggerConfiguration configuration,
        Action<ElasticsearchSinkOptions> options)
    {
        var elasticsearchSinkOptions = configuration.GetElasticsearchSinkOptions();

        if (elasticsearchSinkOptions is null)
            return configuration;

        options(elasticsearchSinkOptions);

        return configuration;
    }
}