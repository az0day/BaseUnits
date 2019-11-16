using System;
using System.Collections.Generic;
using System.Diagnostics;
using BaseUnits.Core.Diagnostics;

namespace BaseUnits.FormExtensions.Diagnostics
{
    /// <summary>
    /// 专属于 Windows Form 程序的性能统计
    /// </summary>
    public class StatsCounterWin : IStatsCounter
    {
        public IPerformanceCounter Counter { get; protected set; }

        public long NextValue => (long)Math.Round(Counter.NextValue());
        public float NextValueFloat => Counter.NextValue();

        public static bool Initialized { get; protected set; }
        public static PerformanceCounterCategory CounterCategory { get; protected set; }

        public static readonly List<CounterCreationData> CounterCreationDataList = new List<CounterCreationData>();

        private static readonly IDictionary<StatCounterType, PerformanceCounterType> counterTypes = new Dictionary<StatCounterType, PerformanceCounterType>
        {
            { StatCounterType.NumberOfItemsHEX32, PerformanceCounterType.NumberOfItemsHEX32 },
            { StatCounterType.NumberOfItemsHEX64, PerformanceCounterType.NumberOfItemsHEX64 },
            { StatCounterType.NumberOfItems32, PerformanceCounterType.NumberOfItems32 },
            { StatCounterType.NumberOfItems64, PerformanceCounterType.NumberOfItems64 },
            { StatCounterType.CounterDelta32, PerformanceCounterType.CounterDelta32 },
            { StatCounterType.CounterDelta64, PerformanceCounterType.CounterDelta64 },
            { StatCounterType.SampleCounter, PerformanceCounterType.SampleCounter },
            { StatCounterType.CountPerTimeInterval32, PerformanceCounterType.CountPerTimeInterval32 },
            { StatCounterType.CountPerTimeInterval64, PerformanceCounterType.CountPerTimeInterval64 },
            { StatCounterType.RateOfCountsPerSecond32, PerformanceCounterType.RateOfCountsPerSecond32 },
            { StatCounterType.RateOfCountsPerSecond64, PerformanceCounterType.RateOfCountsPerSecond64 },
            { StatCounterType.RawFraction, PerformanceCounterType.RawFraction },
            { StatCounterType.CounterTimer, PerformanceCounterType.CounterTimer },
            { StatCounterType.Timer100Ns, PerformanceCounterType.Timer100Ns },
            { StatCounterType.SampleFraction, PerformanceCounterType.SampleFraction },
            { StatCounterType.CounterTimerInverse, PerformanceCounterType.CounterTimerInverse },
            { StatCounterType.Timer100NsInverse, PerformanceCounterType.Timer100NsInverse },
            { StatCounterType.CounterMultiTimer, PerformanceCounterType.CounterMultiTimer },
            { StatCounterType.CounterMultiTimer100Ns, PerformanceCounterType.CounterMultiTimer100Ns },
            { StatCounterType.CounterMultiTimerInverse, PerformanceCounterType.CounterMultiTimerInverse },
            { StatCounterType.CounterMultiTimer100NsInverse, PerformanceCounterType.CounterMultiTimer100NsInverse },
            { StatCounterType.AverageTimer32, PerformanceCounterType.AverageTimer32 },
            { StatCounterType.ElapsedTime, PerformanceCounterType.ElapsedTime },
            { StatCounterType.AverageCount64, PerformanceCounterType.AverageCount64 },
            { StatCounterType.SampleBase, PerformanceCounterType.SampleBase },
            { StatCounterType.AverageBase, PerformanceCounterType.AverageBase },
            { StatCounterType.RawBase, PerformanceCounterType.RawBase },
            { StatCounterType.CounterMultiBase, PerformanceCounterType.CounterMultiBase },
        };

        private static readonly IDictionary<StatCounterCategoryType, PerformanceCounterCategoryType> categoryTypes = new Dictionary<StatCounterCategoryType, PerformanceCounterCategoryType>
        {
            { StatCounterCategoryType.Unknown, PerformanceCounterCategoryType.Unknown },
            { StatCounterCategoryType.MultiInstance, PerformanceCounterCategoryType.MultiInstance },
            { StatCounterCategoryType.SingleInstance, PerformanceCounterCategoryType.SingleInstance },
        };

        public StatsCounterWin(string statsName)
        {
            if (!Initialized)
            {
                throw new Exception("StatsCounter has not been initialized.");
            }

            if (string.IsNullOrWhiteSpace(statsName))
            {
                throw new ArgumentNullException(nameof(statsName));
            }

            var found = false;
            var counters = CounterCategory.GetCounters();
            foreach (var counter in counters)
            {
                if (counter.CounterName != statsName)
                {
                    continue;
                }

                counter.ReadOnly = false;
                counter.RawValue = 0;
                Counter = new PerformanceCounterWin(counter);
                found = true;
            }

            if (!found)
            {
                throw new KeyNotFoundException($"Counter [{statsName}] was not defined and initialized.");
            }
        }

        public void Increment(long value = 1)
        {
            Counter.IncrementBy(value);
        }

        public void Decrement()
        {
            Counter.Decrement();
        }

        public void SetRawValue(long value)
        {
            Counter.SetRawValue(value);
        }

        private static PerformanceCounterType GetCounterType(StatCounterType counterType)
        {
            PerformanceCounterType type;
            if (counterTypes.TryGetValue(counterType, out type))
            {
                return type;
            }

            return PerformanceCounterType.NumberOfItems32;
        }

        private static PerformanceCounterCategoryType GetCategoryType(StatCounterCategoryType categoryType)
        {
            PerformanceCounterCategoryType type;
            if (categoryTypes.TryGetValue(categoryType, out type))
            {
                return type;
            }

            return PerformanceCounterCategoryType.Unknown;
        }

        /// <summary>
        /// Initialize counters for a category
        /// </summary>
        /// <param name="categoryName">i.e. CMDK MiniBetListFeeder or CMDK StakePlaceFeeder</param>
        /// <param name="categoryHelp">MiniBetListFeeder Statistics</param>
        /// <param name="counters">List of statistics name associated with their type</param>
        /// <param name="categoryType"></param>
        public static void Initialize(
            string categoryName,
            string categoryHelp,
            Dictionary<string, StatCounterType> counters,
            StatCounterCategoryType categoryType = StatCounterCategoryType.SingleInstance)
        {
            if (Initialized)
            {
                throw new Exception("StatsCounter has already been initialized.");
            }

            if (string.IsNullOrWhiteSpace(categoryName)) throw new ArgumentNullException(nameof(categoryName));
            if (string.IsNullOrWhiteSpace(categoryHelp)) throw new ArgumentNullException(nameof(categoryHelp));
            if (counters == null) throw new ArgumentNullException(nameof(counters));

            foreach (var counter in counters)
            {
                CounterCreationDataList.Add(
                    new CounterCreationData
                    {
                        CounterName = counter.Key,
                        CounterType = GetCounterType(counter.Value)
                    }
                );
            }

            if (PerformanceCounterCategory.Exists(categoryName))
            {
                // 要删除整块栏目需要管理员权限
                // PerformanceCounterCategory.Delete(categoryName);

                // 使用已建立的栏目
                CounterCategory = new PerformanceCounterCategory(categoryName);
            }
            else
            {
                CounterCategory = PerformanceCounterCategory.Create(
                    categoryName,
                    categoryHelp,
                    GetCategoryType(categoryType),
                    new CounterCreationDataCollection(CounterCreationDataList.ToArray())
                );
            }

            Initialized = true;
        }

        public static void DeleteCategories(IList<string> categories)
        {
            foreach (var category in categories)
            {
                if (PerformanceCounterCategory.Exists(category))
                {
                    PerformanceCounterCategory.Delete(category);
                }
            }
        }
    }

    /// <summary>
    /// Windows Platform
    /// </summary>
    public class PerformanceCounterWin : IPerformanceCounter
    {
        private readonly PerformanceCounter _counter;

        public PerformanceCounterWin(PerformanceCounter counter)
        {
            _counter = counter;
        }

        public long Decrement()
        {
            return _counter.Decrement();
        }

        public long Increment()
        {
            return _counter.Increment();
        }

        public long IncrementBy(long value)
        {
            return _counter.IncrementBy(value);
        }

        public float NextValue()
        {
            return _counter.NextValue();
        }

        public void SetRawValue(long value)
        {
            _counter.RawValue = value;
        }
    }
}
