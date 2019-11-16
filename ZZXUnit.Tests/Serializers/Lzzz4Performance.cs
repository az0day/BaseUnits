using System.Collections.Generic;
using System.Diagnostics;

using Xunit;
using Xunit.Abstractions;
using ZZXUnit.Tests.Entities;
using ZZXUnit.Tests.Entities.SpiderOdds;
using ZZXUnit.Tests.Entities.Subscribers;

namespace ZZXUnit.Tests.Serializers
{
    /// <summary>
    /// LZ4 Tester
    /// https://github.com/MiloszKrajewski/lz4net
    /// </summary>
    public class Lzzz4Performance : BaseSerializerPerformanceTest
    {
        private readonly ITestOutputHelper _output;

        public Lzzz4Performance(ITestOutputHelper outputHelper)
        {
            _output = outputHelper;
        }

        /// <summary>
        /// 2018-08-30 PC/I7
        /// 2 X 4000: 10M/S
        /// 5 X 6000: 10M/S
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="count"></param>
        /// <param name="hdp"></param>
        /// <param name="ou"></param>
        [Theory]
        [InlineData(20, 16, -1607461323900463029, 1984267839953040494)]
        [InlineData(2, 4000, -1607461323900463029, 1984267839953040494)]
        [InlineData(5, 6000, -1607461323900463029, 1984267839953040494)]
        public void Lz4Bytes_Test(int rows, int count, long hdp, long ou)
        {
            var st = Stopwatch.StartNew();
            var pe = CreateEntity(rows, count);
            var ms = st.ElapsedMilliseconds;
            _output.WriteLine(@"init - count: {0}, {1}ms", count, ms);
            _output.WriteLine("");

            var origin = pe.ToBinaryBytes();
            var olength = origin.Length;

            var bytes = new byte[0];
            for (var i = 1; i <= TEST_TIMES; i++)
            {
                st.Restart();
                bytes = pe.ToLz4Bytes();
                ms = st.ElapsedMilliseconds;

                var length = bytes.Length;
                var avg = (olength * 1000M) / (ms * M1);
                _output.WriteLine(@"ToLz4Bytes - length: {0:N0}({4:N2}%), {1:N0}ms, {2:N0}M/S - {3}", length, ms, avg, i, (length * 100M) / olength);
            }

            _output.WriteLine("");
            IDictionary<long, ThirdValueDataBase> prices = new Dictionary<long, ThirdValueDataBase>();
            for (var i = 1; i <= TEST_TIMES; i++)
            {
                st.Restart();
                var oo = bytes.FromLz4Bytes<IList<MatchPriceEntity>>();
                prices = oo[0].Prices;
                ms = st.ElapsedMilliseconds;
                _output.WriteLine(@"FromLz4Bytes: {0:N0}ms - {1}", ms, i);
            }

            _output.WriteLine("");
            var ohdp = (ThirdValueHdp)prices[hdp];
            _output.WriteLine(@"Category: {0}, Home: {1}", ohdp.Category, ohdp.Home);

            Assert.Equal(ThirdDataCategoryEnum.Hdp, ohdp.Category);
            Assert.Equal(0.80m, ohdp.Home);

            var oou = (ThirdValueOu)prices[ou];
            _output.WriteLine(@"Category: {0}, Home: {1}", oou.Category, oou.Over);

            Assert.Equal(ThirdDataCategoryEnum.Ou, oou.Category);
            Assert.Equal(0.90m, oou.Over);
        }
    }

}
