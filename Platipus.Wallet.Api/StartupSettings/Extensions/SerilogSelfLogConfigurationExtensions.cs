namespace Platipus.Wallet.Api.StartupSettings.Extensions;

using Serilog;
using Serilog.Debugging;

public static class SerilogSelfLogConfigurationExtensions
{
    public static LoggerConfiguration EnableSelfLog(this LoggerConfiguration configuration, HostBuilderContext context)
    {
        SelfLog.Enable(
            m =>
            {
                var log = $"{m}\n";

                if (context.HostingEnvironment.IsDebug())
                    Console.Error.WriteLine(log);

                var path = context.Configuration
                               .GetSection("Serilog")
                               .GetValue<string>("SelfLogFilePath") ??
                           "./self.txt";

                File.AppendAllText(path, log);
            });

        return configuration;
    }
}