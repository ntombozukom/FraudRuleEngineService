namespace FraudEngine.API.Constants;

public static class ConfigurationKeys
{
    public const string DefaultConnection = "DefaultConnection";

    public static class DatabaseStartup
    {
        public const string Section = "DatabaseStartup";
        public const string MaxRetries = $"{Section}:MaxRetries";
        public const string RetryDelaySeconds = $"{Section}:RetryDelaySeconds";
    }
}
