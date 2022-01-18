﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace LowKey.Data
{
    public interface ITenantedCommandSession<TClient>
    {
        Task Execute(DataStoreId dataStoreId, Tenant tenant, Func<TClient, Task> command, CancellationToken cancellation = default);
    }
}