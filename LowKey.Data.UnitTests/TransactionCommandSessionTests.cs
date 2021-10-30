using AutoFixture.Xunit2;
using LowKey.Data.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class TransactionCommandSessionTests
    {
        ICommandSession<TestClient> _commandSession;
        public TransactionCommandSessionTests()
        {
            _commandSession = new TransactionalCommandSession<TestClient>(new TestCommandSession(), TransactionScopeOption.Required, new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            });
        }

        [Fact]
        public async Task TransactionIsPresentWithinCommandHandler()
        {
            await _commandSession.Execute(new Db("test", "", 0), client =>
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

            return _commandSession.Execute(new Db("test", "", 0), client =>
            {
                Assert.Equal(transactionOptions.IsolationLevel, Transaction.Current?.IsolationLevel);
                return Task.CompletedTask;
            });
        }

        [Fact]
        public async Task TransactionIsCommitted()
        {
            var transactionScopeOptions = TransactionScopeOption.Required;
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.MaxValue
            };

            _commandSession = new TransactionalCommandSession<TestClient>(new TestCommandSession(), transactionScopeOptions, transactionOptions);

            Transaction transaction;
                        
            await _commandSession.Execute(new Db("test", "", 0), client =>
            {
                transaction = Transaction.Current;
                return Task.CompletedTask;
            });

            Assert.Equal(TransactionStatus.Committed, TransactionStatus.Committed);
        }
    }

    class TestCommandSession : ICommandSession<TestClient>
    {
        public Task Execute(Db db, Func<TestClient, Task> command, CancellationToken cancellation = default)
        {
            return Task.CompletedTask;
        }
    }
}
