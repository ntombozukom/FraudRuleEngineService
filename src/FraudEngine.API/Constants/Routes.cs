namespace FraudEngine.API.Constants;

public static class Routes
{
    private const string ApiBase = "/api";

    public const string Transactions = $"{ApiBase}/transactions";
    public const string FraudAlerts = $"{ApiBase}/fraud-alerts";
    public const string FraudRules = $"{ApiBase}/fraud-rules";
    public const string AuditLogs = $"{ApiBase}/audit-logs";

    public static class TransactionRoutes
    {
        public const string Evaluate = "/evaluate";
    }

    public static class FraudAlertRoutes
    {
        public const string ByReference = "/{alertReference:guid}";
        public const string Review = "/{alertReference:guid}/review";
        public const string Statistics = "/statistics";
    }

    public static class FraudRuleRoutes
    {
        public const string ByName = "/{ruleName}";
    }
}
