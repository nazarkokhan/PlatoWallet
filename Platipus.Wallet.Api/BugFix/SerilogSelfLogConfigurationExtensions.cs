// namespace Platipus.Wallet.Api.BugFIx;
//
// using Extensions;
// using Serilog;
// using Serilog.Debugging;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Hosting;
//
// public static class SerilogSelfLogConfigurationExtensions
// {
//     private static string _filePath = "./self.txt";
//
//     public static LoggerConfiguration EnableSelfLog(this LoggerConfiguration configuration, HostBuilderContext context)
//     {
//         var configuredFilePath = context.Configuration
//             .GetSection("Serilog")
//             .GetValue<string>("SelfLogFilePath");
//
//         if (configuredFilePath is not null)
//             _filePath = configuredFilePath;
//
//         Action<string> logAction = m => File.AppendAllText(_filePath, $"{m}\n");
//         
//         if (context.HostingEnvironment.IsDebug())
//             logAction += m => Console.Error.WriteLine($"{m}\n");
//         
//         return configuration.EnableSelfLog(logAction);
//     }
//
//     public static LoggerConfiguration EnableSelfLog(this LoggerConfiguration configuration, Action<string> output)
//     {
//         SelfLog.Enable(output);
//
//         return configuration;
//     }
// }