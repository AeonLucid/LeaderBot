using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

namespace LeaderBot.Config
{
    internal static class NLogConfig
    {
        public static LoggingConfiguration Create(bool isDev)
        {
            var config = new LoggingConfiguration();

            config.AddTarget(new ColoredConsoleTarget
            {
                Name = "BeautifulOutput",
                Layout = Layout.FromString("${longdate} ${pad:padding=5:inner=${level:uppercase=true}} ${message} ${exception:format=tostring}"),
                UseDefaultRowHighlightingRules = true,
                ErrorStream = false
            });

            if (isDev)
            {
                config.AddRule(LogLevel.Trace, LogLevel.Off, "BeautifulOutput");
            }
            else
            {
                config.AddRule(LogLevel.Info, LogLevel.Fatal, "BeautifulOutput");
            }

            return config;
        }
    }
}