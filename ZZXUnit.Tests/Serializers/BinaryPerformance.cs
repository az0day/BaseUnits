using System.Collections.Generic;
using System.Diagnostics;

using Xunit;
using Xunit.Abstractions;

using BaseUnits.Core.Helpers;
using ZZXUnit.Tests.Entities;
using ZZXUnit.Tests.Entities.SpiderOdds;
using ZZXUnit.Tests.Entities.Subscribers;

namespace ZZXUnit.Tests.Serializers
{
    public class BinaryPerformance : BaseSerializerPerformanceTest
    {
        private readonly ITestOutputHelper _output;

        public BinaryPerformance(ITestOutputHelper outputHelper)
        {
            _output = outputHelper;
        }

        /// <summary>
        /// 2018-08-30 PC/I7
        /// 2 X 4000: 11M/S
        /// 5 X 6000: 11M/S
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="count"></param>
        /// <param name="hdp"></param>
        /// <param name="ou"></param>
        [Theory]
        [InlineData(20, 16, -1607461323900463029, 1984267839953040494)]
        [InlineData(2, 4000, -1607461323900463029, 1984267839953040494)]
        [InlineData(5, 6000, -1607461323900463029, 1984267839953040494)]
        public void Binary_Test(int rows, int count, long hdp, long ou)
        {
            var st = Stopwatch.StartNew();
            var pe = CreateEntity(rows, count);

            var ms = st.ElapsedMilliseconds;
            _output.WriteLine(@"init - count: {0}, {1}ms", count, ms);

            var ohs = pe[0].Prices[hdp].ToJsonString();
            var oos = pe[0].Prices[ou].ToJsonString();

            var bytes = new byte[0];
            for (var i = 1; i <= TEST_TIMES; i++)
            {
                st.Restart();
                bytes = pe.ToBinaryBytes();
                ms = st.ElapsedMilliseconds;

                var length = bytes.Length;
                var avg = (length * 1000M) / (ms * M1);
                _output.WriteLine(@"ToBinaryBytes - length: {0:N0}, {1:N0}ms, {2:N0}M/S - {3}", length, ms, avg, i);
            }

            _output.WriteLine("");
            IDictionary<long, ThirdValueDataBase> prices = new Dictionary<long, ThirdValueDataBase>();
            for (var i = 1; i <= TEST_TIMES; i++)
            {
                st.Restart();
                var oo = bytes.FromBinaryBytes<IList<MatchPriceEntity>>();
                prices = oo[0].Prices;
                ms = st.ElapsedMilliseconds;
                _output.WriteLine(@"FromBinaryBytes: {0:N0}ms - {1}", ms, i);
            }

            _output.WriteLine("");
            var nhs = prices[hdp].ToJsonString();
            var nos = prices[ou].ToJsonString();

            Assert.Equal(ohs, nhs);
            Assert.Equal(oos, nos);

            var ohdp = (ThirdValueHdp)prices[hdp];
            _output.WriteLine(@"Category: {0}, Home: {1}", ohdp.Category, ohdp.Home);

            Assert.Equal(ThirdDataCategoryEnum.Hdp, ohdp.Category);
            Assert.Equal(0.80m, ohdp.Home);

            var oou = (ThirdValueOu)prices[ou];
            _output.WriteLine(@"Category: {0}, Home: {1}", oou.Category, oou.Over);

            Assert.Equal(ThirdDataCategoryEnum.Ou, oou.Category);
            Assert.Equal(0.90m, oou.Over);
        }

        /// <summary>
        /// 2018-08-30 PC/I7
        /// 2 X 4000: 8M/S
        /// 5 X 6000: 8M/S
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="count"></param>
        /// <param name="hdp"></param>
        /// <param name="ou"></param>
        [Theory]
        [InlineData(20, 16, -1607461323900463029, 1984267839953040494)]
        [InlineData(2, 4000, -1607461323900463029, 1984267839953040494)]
        [InlineData(5, 6000, -1607461323900463029, 1984267839953040494)]
        public void GZip_Test(int rows, int count, long hdp, long ou)
        {
            var st = Stopwatch.StartNew();
            var pe = CreateEntity(rows, count);

            var ms = st.ElapsedMilliseconds;
            _output.WriteLine(@"init - count: {0}, {1}ms", count, ms);

            var origin = pe.ToBinaryBytes();
            var olength = origin.Length;

            var ohs = pe[0].Prices[hdp].ToJsonString();
            var oos = pe[0].Prices[ou].ToJsonString();

            var bytes = new byte[0];
            for (var i = 1; i <= TEST_TIMES; i++)
            {
                st.Restart();
                bytes = BinaryHelper.ToBytes(pe, true);
                ms = st.ElapsedMilliseconds;

                var length = bytes.Length;
                var avg = (olength * 1000M) / (ms * M1);
                _output.WriteLine(@"ToSnappyBytes - length: {0:N0}({4:N2}%), {1:N0}ms, {2:N0}M/S - {3}", length, ms, avg, i, (length * 100M) / olength);
            }

            _output.WriteLine("");
            IDictionary<long, ThirdValueDataBase> prices = new Dictionary<long, ThirdValueDataBase>();
            for (var i = 1; i <= TEST_TIMES; i++)
            {
                st.Restart();
                var oo = BinaryHelper.FromBytes<IList<MatchPriceEntity>>(bytes, true);
                prices = oo[0].Prices;
                ms = st.ElapsedMilliseconds;
                _output.WriteLine(@"FromGZipBytes: {0:N0}ms - {1}", ms, i);
            }

            _output.WriteLine("");
            var nhs = prices[hdp].ToJsonString();
            var nos = prices[ou].ToJsonString();

            Assert.Equal(ohs, nhs);
            Assert.Equal(oos, nos);

            var ohdp = (ThirdValueHdp)prices[hdp];
            _output.WriteLine(@"Category: {0}, Home: {1}", ohdp.Category, ohdp.Home);

            Assert.Equal(ThirdDataCategoryEnum.Hdp, ohdp.Category);
            Assert.Equal(0.80m, ohdp.Home);

            var oou = (ThirdValueOu)prices[ou];
            _output.WriteLine(@"Category: {0}, Home: {1}", oou.Category, oou.Over);

            Assert.Equal(ThirdDataCategoryEnum.Ou, oou.Category);
            Assert.Equal(0.90m, oou.Over);
        }

        /// <summary>
        /// 2018-08-30 PC/I7
        /// 2 X 4000: 8M/S
        /// 5 X 6000: 8M/S
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="count"></param>
        /// <param name="hdp"></param>
        /// <param name="ou"></param>
        [Theory]
        [InlineData(20, 16, -1607461323900463029, 1984267839953040494)]
        [InlineData(2, 4000, -1607461323900463029, 1984267839953040494)]
        [InlineData(5, 6000, -1607461323900463029, 1984267839953040494)]
        public void Deflate_Test(int rows, int count, long hdp, long ou)
        {
            var st = Stopwatch.StartNew();
            var pe = CreateEntity(rows, count);

            var ms = st.ElapsedMilliseconds;
            _output.WriteLine(@"init - count: {0}, {1}ms", count, ms);
            _output.WriteLine("");

            var origin = pe.ToBinaryBytes();
            var olength = origin.Length;

            var ohs = pe[0].Prices[hdp].ToJsonString();
            var oos = pe[0].Prices[ou].ToJsonString();

            var bytes = new byte[0];
            for (var i = 1; i <= TEST_TIMES; i++)
            {
                st.Restart();
                bytes = BinaryHelper.ToBytesDeflate(pe);
                ms = st.ElapsedMilliseconds;

                var length = bytes.Length;
                var avg = (olength * 1000M) / (ms * M1);
                _output.WriteLine(@"ToBytesDeflate - length: {0:N0}({4:N2}%), {1:N0}ms, {2:N0}M/S - {3}", length, ms, avg, i, (length * 100M) / olength);
            }

            _output.WriteLine("");
            IDictionary<long, ThirdValueDataBase> prices = new Dictionary<long, ThirdValueDataBase>();
            for (var i = 1; i <= TEST_TIMES; i++)
            {
                st.Restart();
                var oo = BinaryHelper.FromBytesDeflate<IList<MatchPriceEntity>>(bytes);
                prices = oo[0].Prices;
                ms = st.ElapsedMilliseconds;
                _output.WriteLine(@"FromBytesDeflate: {0:N0}ms - {1}", ms, i);
            }

            _output.WriteLine("");
            var nhs = prices[hdp].ToJsonString();
            var nos = prices[ou].ToJsonString();

            Assert.Equal(ohs, nhs);
            Assert.Equal(oos, nos);

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
