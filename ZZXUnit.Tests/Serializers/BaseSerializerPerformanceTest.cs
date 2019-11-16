using System;
using System.Collections.Generic;

using BaseUnits.Core.Helpers;
using ZZXUnit.Tests.Entities.SpiderOdds;
using ZZXUnit.Tests.Entities.Subscribers;

namespace ZZXUnit.Tests.Serializers
{
    public abstract class BaseSerializerPerformanceTest
    {
        protected const int TEST_TIMES = 5;
        protected const decimal M1 = 1048576M;

        protected IList<MatchPriceEntity> CreateEntity(int rows, int count)
        {
            var list = new List<MatchPriceEntity>();
            for (var j = 0; j < rows; j++)
            {
                var max = Math.Ceiling(count / 4m);
                var pe = new MatchPriceEntity
                {
                    //PostTime = StaticHelper.NowTime,
                    //LastUpdate = StaticHelper.DateTimeMin,
                    Version = StaticHelper.GetTimestamp(StaticHelper.NowTime),

                    Match = new ThirdPartyMatchRelation(),
                    Prices = new Dictionary<long, ThirdValueDataBase>(),
                };

                for (var i = 0; i < count; i++)
                {
                    var d = new ThirdValueHdp
                    {
                        FirstHalf = false,
                        Hdp = max - 0.25m * i,
                        Home = 0.80m,
                        Away = 0.86m,
                    };

                    var hash = d.CreateOwnHash();
                    d.OwnHash = hash;
                    pe.Prices.Add(hash, d);
                }

                for (var i = 0; i < count; i++)
                {
                    var d = new ThirdValueOu
                    {
                        FirstHalf = false,
                        Ou = 0.25m + 0.25m * i,
                        Over = 0.90m,
                        Under = 0.86m,
                    };

                    var hash = d.CreateOwnHash();
                    d.OwnHash = hash;
                    pe.Prices.Add(hash, d);
                }

                list.Add(pe);
            }
            return list;
        }
    }
}
