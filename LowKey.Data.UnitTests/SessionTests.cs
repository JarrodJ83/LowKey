using System.Threading.Tasks;
using Xunit;

namespace LowKey.Data.UnitTests
{
    public class SessionTests
    {
        DataStoreId TestDataStore = new DataStoreId("Test");
        TenantedSession<TestClient> _session;

        public SessionTests()
        { }
        record TestResult;
    }
}
