﻿using LowKey.Data;
using LowKey.Data.Sql;
using LowKey.Extensions.Hosting;
using System.Data.Common;
using System.Data.SqlClient;

namespace Microsoft.Extensions.Hosting
{
    public static class LowKeyDataStoreConfigExtensions
    {
        public static LowKeyDataStoreConfig WithSqlServer(this LowKeyDataStoreConfig config, SqlConnectionStringBuilder connectionStringBuilder)
        {
            config.UseClientFactory(() => (IClientFactory<DbConnection>)new SqlDbConnectionFactory(connectionStringBuilder));
            config.UseClientFactory(() => (IClientFactory<SqlConnection>)new SqlDbConnectionFactory(connectionStringBuilder));

            return config;
        }
    }
}
