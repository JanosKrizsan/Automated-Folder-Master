using System;
using NLog;
using NLog.Config;
using NLog.Targets;


namespace Master_Console.Services
{
    public static class ErrorHandlingService
    {
        public static void ExceptionHandler(Exception ex)
        {
            var errorMessage = "";
            var level = LogLevel.Info;

            switch (ex.GetType().ToString())
            {
                case "System.IO.FileNotFoundException":
                    errorMessage = "The settings file cannot be found, please set up the program before running.";
                    level = LogLevel.Error;
                    break;
                case "System.IO.InvalidDataException":
                    errorMessage = "There are no directories selected, please provide them via the settings app.";
                    level = LogLevel.Warn;
                    break;
                case "System.IO.PathTooLongException":
                    errorMessage = "The path provided is too long, or invalid.";
                    level = LogLevel.Warn;
                    break;
                case "System.ArgumentException":
                    errorMessage = "There are missing settings, or they were provided incorrectly.";
                    level = LogLevel.Warn;
                    break;
                case "System.ArgumentNullException":
                    errorMessage = "There are missing settings, or the resources do not exist anymore.";
                    level = LogLevel.Warn;
                    break;
                case "System.FormatException":
                case "System.NotSupportedException":
                    errorMessage = "The given path or value is not supported.";
                    level = LogLevel.Warn;
                    break;
                case "System.TypeInitializationException":
                    errorMessage = "A fatal error occurred, please contact the programmer.";
                    level = LogLevel.Fatal;
                    break;
                case "System.Exception":
                    errorMessage = "An unknown error occurred, please do something useful and contact the programmer.";
                    level = LogLevel.Fatal;
                    break;
            }

            LoggingManager.Log(level, ex, errorMessage);
        }

        public static void Shutdown()
        {
            LoggingManager.ShutdownLogger();
        }

        internal static class LoggingManager
        {
            private static LoggingConfiguration _config = new LoggingConfiguration();
            private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            private static bool _configured = false;

            private static void Configure()
            {
                var fileTarget = new FileTarget()
                {
                    ArchiveOldFileOnStartup = false,
                    FileName = "${basedir}/file.txt"
                };

                var consoleTarget = new ColoredConsoleTarget()
                {
                    Name = "console",
                };

                _config.AddTarget("LogFile", fileTarget);
                _config.AddTarget("console", consoleTarget);

                _configured = true;
            }

            public static void Log(LogLevel level, Exception ex, string message)
            {
                if (!_configured)
                {
                    Configure();
                }

                _logger.Log(level, ex, message);
            }

            public static void ShutdownLogger()
            {
                LogManager.Shutdown();
            }
        }
    }
}
