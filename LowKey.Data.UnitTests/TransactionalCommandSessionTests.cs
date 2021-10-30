using AutoFixture.Xunit2;
using LowKey.Data.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class TransactionalCommandSessionTests
    {
        TransactionalCommandSession<TestClient> _commandSession;
        TestCommandSession _decoratedCommandSession;
        Db TestDb = new("TestDb", "test.server", 0);

        public TransactionalCommandSessionTests()
        {
            _decoratedCommandSession = new TestCommandSession();
            _commandSession = new TransactionalCommandSession<TestClient>(_decoratedCommandSession, TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            });
        }

        [Fact]
        public async Task TransactionIsPresentWithinCommandHandler()
        {
            await _commandSession.Execute(TestDb, client =>
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

            _commandSession = new TransactionalCommandSession<TestClient>(new TestCommandSession(), transactionScopeOptions, transactionOptions);

            return _commandSession.Execute(TestDb, client =>
            {
                Assert.Equal(transactionOptions.IsolationLevel, Transaction.Current?.IsolationLevel);
                return Task.CompletedTask;
            });
        }

        [Fact]
        public async Task TransactionIsCommitted()
        {
            TransactionStatus? transactionStatus = null;

            await _commandSession.Execute(TestDb, client =>
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
            await _commandSession.Execute(TestDb, client =>
            {
                return Task.CompletedTask;
            });

            Assert.True(_decoratedCommandSession.Executed);
        }
    }

    class TestCommandSession : ICommandSession<TestClient>
    {
        public bool Executed = false;

        public Task Execute(Db db, Func<TestClient, Task> command, CancellationToken cancellation = default)
        {
            Executed = true;
            return command(new TestClient(db));
        }
    }
}
