using LowKey.Data.MultiTenancy.Transactions;

namespace LowKey.Extensions.Hosting
{
    public class LowKeyDataOptions
    {
        public bool EnableDiagnosticActivities { get; set; }
        public CommandOptions CommandOptions { get; set; } = new CommandOptions();
    }

    public class CommandOptions
    {
        public TransactionSettings? TransactionSettings { get; set; }

        public CommandOptions()
        {
        }

        public CommandOptions(TransactionSettings transactionSettings)
        {
            TransactionSettings = transactionSettings;
        }
    }
}
