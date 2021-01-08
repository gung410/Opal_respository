using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Microservice.Badge.Infrastructure.Helpers
{
    public static class HangfireHelper
    {
        public static async Task PerformBatchJob<T>(IAggregateFluent<T> aggregateFluent, Action<List<T>> batchJobExecutionFn, CancellationToken cancellationToken = default)
        {
            using (var cursor = await aggregateFluent.ToCursorAsync(cancellationToken))
            {
                while (await cursor.MoveNextAsync(cancellationToken))
                {
                    var items = cursor.Current.ToList();
                    batchJobExecutionFn(items);
                }
            }
        }
    }
}
