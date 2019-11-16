using System.Collections.Generic;
using System.Diagnostics;
using BaseUnits.Serializers;
using BaseUnits.Serializers.Compress;
using Xunit;
using Xunit.Abstractions;
using ZZXUnit.Tests.Entities.SpiderOdds;
using ZZXUnit.Tests.Entities.Subscribers;

namespace ZZXUnit.Tests.Serializers
{
    /// <summary>
    /// A fast object graph binary serialization for .NET
    /// https://github.com/Suremaker/ObjectSerialization-Net
    /// </summary>
    public class ObjectSerializerPerformance : BaseSerializerPerformanceTest
    {
        private readonly ITestOutputHelper _output;
        private readonly ISerializer _serializer;

        public ObjectSerializerPerformance(ITestOutputHelper outputHelper)
        {
            _output = outputHelper;
            _serializer = new ObjectSerializer();
        }

        /// <summary>
        /// 2018-08-30 failed
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="count"></param>
        /// <param name="hdp"></param>
        /// <param name="ou"></param>
        [Theory]
        [InlineData(1, 20000, -1607461323900463029, 1984267839953040494)]
        [InlineData(2, 4000, -1607461323900463029, 1984267839953040494)]
        [InlineData(5, 6000, -1607461323900463029, 1984267839953040494)]
        public void ObjectSerializationBytes_Test(int rows, int count, long hdp, long ou)
        {
            var st = Stopwatch.StartNew();
            var pe = CreateEntity(rows, count);
            var ms = st.ElapsedMilliseconds;
            _output.WriteLine(@"init - count: {0}, {1}ms", count, ms);
            _output.WriteLine("");

            var bytes = new byte[0];
            for (var i = 1; i <= TEST_TIMES; i++)
            {
                st.Restart();
                bytes = _serializer.ToBytes(pe);
                ms = st.ElapsedMilliseconds;

                var length = bytes.Length;
                var avg = (length / (ms * 1000));
                _output.WriteLine(@"ToObjectSerializerBytes - length: {0:N0}, {1:N0}ms, {2:N0}M/S - {3}", length, ms, avg, i);
            }

            _output.WriteLine("");
            IDictionary<long, ThirdValueDataBase> prices = new Dictionary<long, ThirdValueDataBase>();
            for (var i = 1; i <= TEST_TIMES; i++)
            {
                st.Restart();
                var oo = _serializer.FromBytes<IList<MatchPriceEntity>>(bytes);
                prices = oo[0].Prices;
                ms = st.ElapsedMilliseconds;
                _output.WriteLine(@"FromObjectSerializerBytes: {0:N0}ms - {1}", ms, i);
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
