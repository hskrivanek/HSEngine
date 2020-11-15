namespace HSEngine.Logging
{
    public static class Log
    {
        public static Logger CoreLogger { get; private set; }
        public static Logger ClientLogger { get; private set; }

        public static void Init()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logConsole = new NLog.Targets.ColoredConsoleTarget
            {
                Layout = "${time} | ${level: uppercase = true} | ${logger} | ${message}"
            };
            config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Fatal, logConsole);
            NLog.LogManager.Configuration = config;

            CoreLogger = new Logger("HSE");
            ClientLogger = new Logger("APP");
        }
    }
}
