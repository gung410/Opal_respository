using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LearnerApp.Common.TaskController
{
    public class ParallelTaskRunner
    {
        private readonly int _maxDegreeOfParallelism;

        public ParallelTaskRunner(int maxDegreeOfParallelism = 5)
        {
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        public async Task RunAsync(params Func<Task>[] tasks)
        {
            var tcs = new TaskCompletionSource<object>();
            SemaphoreSlim countSemaphore = new SemaphoreSlim(1);
            int count = 0;
            int taskCount = tasks.Length;
            Parallel.ForEach(
                tasks,
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = _maxDegreeOfParallelism,
                },
                async (task) =>
                {
                    var exceptions = new List<Exception>();
                    try
                    {
                        await task();
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e);
                    }

                    try
                    {
                        await countSemaphore.WaitAsync();
                        count++;
                        if (count == taskCount)
                        {
                            if (exceptions.Any())
                            {
                                tcs.SetException(new AggregateException(exceptions));
                            }
                            else
                            {
                                tcs.SetResult(default(object));
                            }
                        }
                    }
                    finally
                    {
                        countSemaphore.Release();
                    }
                });

            await tcs.Task;
        }
    }
}
