using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;
using BaseUnits.Core.Helpers;

namespace ZZXUnit.Tests.Performance
{
    public class ThreadQueueActionTests
    {
        private readonly ITestOutputHelper _output;

        public ThreadQueueActionTests(ITestOutputHelper outputHelper)
        {
            _output = outputHelper;
        }


        [Theory]
        [InlineData(500000)]
        [InlineData(1000000)]
        [InlineData(5000000)]
        [InlineData(10000000)]
        public void Single_Tests(int total)
        {
            var autoReset = new AutoResetEvent(false);
            var executor = new ThreadQueueAction();
            var st = Stopwatch.StartNew();

            var sum = 0;
            Parallel.For(0, total, new ParallelOptions { MaxDegreeOfParallelism = 32 }, i =>
            {
                executor.Push(()=>
                {
                    var current = Interlocked.Increment(ref sum);
                    if (current >= total)
                    {
                        autoReset.Set();
                    }
                });
            });

            using (executor)
            {
                autoReset.WaitOne();

                var ms = st.ElapsedMilliseconds;
                var psec = (total * 1000m) / ms;
                _output.WriteLine("task times: {0} - sum: {1}, {2}ms - {3:N} / sec", total, sum, ms, psec);
            }
        }
    }
}
