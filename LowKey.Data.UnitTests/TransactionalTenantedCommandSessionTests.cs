using LowKey.Data.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class TransactionalTenantedCommandSessionTests
    {
        TransactionalTenantedCommandSession<TestClient> _commandSession;
        TestCommandSession _decoratedCommandSession;
        Tenant TestTenant = new("TestDb", "test.server", 0);
        DataStoreId TestDataStore = new("Test");

        public TransactionalTenantedCommandSessionTests()
        {
            _decoratedCommandSession = new TestCommandSession();
            _commandSession = new TransactionalTenantedCommandSession<TestClient>(_decoratedCommandSession, new TransactionSettings(TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }));
        }

        [Fact]
        public async Task TransactionIsPresentWithinCommandHandler()
        {
            await _commandSession.Execute(TestDataStore, TestTenant, client =>
            {
                Assert.NotNull(Transaction.Current);
                return Task.CompletedTask;
            });
        }

        [Fact]
        public Task TransactionHasCorrectSettings()
        {
            var transactionScopeOptions = TransactionScopeOption.Required;
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.MaxValue
            };

            _commandSession = new TransactionalTenantedCommandSession<TestClient>(new TestCommandSession(), new TransactionSettings(transactionScopeOptions, transactionOptions));

            return _commandSession.Execute(TestDataStore, TestTenant, client =>
            {
                Assert.Equal(transactionOptions.IsolationLevel, Transaction.Current?.IsolationLevel);
                return Task.CompletedTask;
            });
        }

        [Fact]
        public async Task TransactionIsCommitted()
        {
            TransactionStatus? transactionStatus = null;

            await _commandSession.Execute(TestDataStore, TestTenant, client =>
            {
                if (Transaction.Current is not null)
                {
                    Transaction.Current.TransactionCompleted += (sender, e) =>
                    {
                        transactionStatus = e.Transaction?.TransactionInformation.Status;
                    };
                }
                return Task.CompletedTask;
            });

            Assert.NotNull(transactionStatus);
            Assert.Equal(TransactionStatus.Committed, transactionStatus);
        }


        [Fact]
        public async Task DecoratedCommandSessionIsInvoked()
        {
            await _commandSession.Execute(TestDataStore, TestTenant, client => Task.CompletedTask);

            Assert.True(_decoratedCommandSession.Executed);
        }
    }

    class TestCommandSession : ITenantedCommandSession<TestClient>
    {
        public bool Executed = false;

        public Task Execute(DataStoreId dataStoreId, Tenant tenant, Func<TestClient, Task> command, CancellationToken cancellation = default)
        {
            Executed = true;
            return command(new TestClient(dataStoreId, tenant));
        }
    }
}
