using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace LowKey.Data.Transactions
{
    public class TransactionSettings
    {
        public TransactionScopeOption TransactionScopeOption;
        public TransactionOptions TransactionOptions;

        public TransactionSettings(TransactionScopeOption transactionScopeOption, TransactionOptions transactionOptions)
        {
            TransactionScopeOption = transactionScopeOption;
            TransactionOptions = transactionOptions;
        }        
    }

    public class TransactionalCommandSession<TClient> : ICommandSession<TClient>
    {
        private readonly TransactionSettings _settings;
        private readonly ICommandSession<TClient> _commandSession;
        
        public TransactionalCommandSession(ICommandSession<TClient> commandSession, TransactionSettings settings)
        {
            _settings = settings;
            _commandSession = commandSession;
        }

        public async Task Execute(DataStoreId dataStoreId, Tenant tenant, Func<TClient, Task> command, CancellationToken cancellation = default)
        {
            using var trxScope = new TransactionScope(_settings.TransactionScopeOption, _settings.TransactionOptions, TransactionScopeAsyncFlowOption.Enabled);
            await _commandSession.Execute(dataStoreId, tenant, command, cancellation);
            trxScope.Complete();
        }
    }
}
