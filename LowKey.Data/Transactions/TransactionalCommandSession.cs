using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace LowKey.Data.Transactions
{
    public class TransactionalCommandSession<TClient> : ICommandSession<TClient>
    {
        private readonly ICommandSession<TClient> _commandSession;
        private readonly TransactionScopeOption _transactionScopeOption;
        private readonly TransactionOptions _transactionOptions;

        public TransactionalCommandSession(ICommandSession<TClient> commandSession, TransactionScopeOption transactionScopeOption, TransactionOptions transactionOptions)
        {
            _commandSession = commandSession;
            _transactionOptions = transactionOptions;
            _transactionScopeOption = transactionScopeOption;
        }

        public async Task Execute(Db db, Func<TClient, Task> command, CancellationToken cancellation = default)
        {
            using var trxScope = new TransactionScope(_transactionScopeOption, _transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
            await _commandSession.Execute(db, command, cancellation);
        }
    }
}
