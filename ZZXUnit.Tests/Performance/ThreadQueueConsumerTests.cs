using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;
using BaseUnits.Core.Helpers;

namespace ZZXUnit.Tests.Performance
{
    public class ThreadQueueConsumerTests
    {
        private long _sum;

        private readonly ITestOutputHelper _output;

        public ThreadQueueConsumerTests(ITestOutputHelper outputHelper)
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
            var executor = new ThreadQueueConsumer<MyClass>(Execute);
            var st = Stopwatch.StartNew();

            var d0 = new MyClass
            {
                Are = autoReset,
                Total = total,
            };
            _sum = 0;

            Parallel.For(0, total, new ParallelOptions { MaxDegreeOfParallelism = 16 }, i =>
            {
                executor.Push(d0);
            });

            using (executor)
            {
                autoReset.WaitOne();

                var ms = st.ElapsedMilliseconds;
                var psec = (total * 1000m) / ms;
                _output.WriteLine("task times: {0} - sum: {1}, {2}ms - {3:N} / sec", total, _sum, ms, psec);
            }
        }

        private void Execute(MyClass d)
        {
            Interlocked.Increment(ref _sum);
            if (_sum == d.Total)
            {
                d.Are.Set();
                _output.WriteLine(@"total: {0}, sum: {1}", d.Total, _sum);
            }

            if (_sum > d.Total)
            {
                Assert.True(false);
            }
        }

        [Serializable]
        public class MyClass
        {
            public AutoResetEvent Are { get; set; }
            public int Total { get; set; }
        }
    }
}
