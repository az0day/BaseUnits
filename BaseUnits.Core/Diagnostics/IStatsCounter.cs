namespace BaseUnits.Core.Diagnostics
{
    /// <summary>
    /// IStatsCounter
    /// </summary>
    public interface IStatsCounter
    {
        IPerformanceCounter Counter { get; }

        long NextValue { get; }
        float NextValueFloat { get; }

        void Increment(long value = 1);
        void Decrement();
        void SetRawValue(long value);
    }

    /// <summary>
    /// - PerformanceCounterCategoryType
    /// Indicates whether the performance counter category can have multiple instances.
    /// </summary>
    public enum StatCounterCategoryType
    {
        /// <summary>
        /// The instance functionality for the performance counter category is unknown.
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// The performance counter category can have only a single instance.
        /// </summary>
        SingleInstance = 0,

        /// <summary>
        /// The performance counter category can have multiple instances.
        /// </summary>
        MultiInstance = 1
    }

    /// <summary>
    /// - PerformanceCounterType
    /// </summary>
    public enum StatCounterType
    {
        //
        // 摘要:
        //     An instantaneous counter that shows the most recently observed value in hexadecimal
        //     format. Used, for example, to maintain a simple count of items or operations.
        NumberOfItemsHEX32 = 0,

        //
        // 摘要:
        //     An instantaneous counter that shows the most recently observed value. Used, for
        //     example, to maintain a simple count of a very large number of items or operations.
        //     It is the same as NumberOfItemsHEX32 except that it uses larger fields to accommodate
        //     larger values.
        NumberOfItemsHEX64 = 256,

        //
        // 摘要:
        //     An instantaneous counter that shows the most recently observed value. Used, for
        //     example, to maintain a simple count of items or operations.
        NumberOfItems32 = 65536,

        //
        // 摘要:
        //     An instantaneous counter that shows the most recently observed value. Used, for
        //     example, to maintain a simple count of a very large number of items or operations.
        //     It is the same as NumberOfItems32 except that it uses larger fields to accommodate
        //     larger values.
        NumberOfItems64 = 65792,

        //
        // 摘要:
        //     A difference counter that shows the change in the measured attribute between
        //     the two most recent sample intervals.
        CounterDelta32 = 4195328,

        //
        // 摘要:
        //     A difference counter that shows the change in the measured attribute between
        //     the two most recent sample intervals. It is the same as the CounterDelta32 counter
        //     type except that is uses larger fields to accomodate larger values.
        CounterDelta64 = 4195584,

        //
        // 摘要:
        //     An average counter that shows the average number of operations completed in one
        //     second. When a counter of this type samples the data, each sampling interrupt
        //     returns one or zero. The counter data is the number of ones that were sampled.
        //     It measures time in units of ticks of the system performance timer.
        SampleCounter = 4260864,

        //
        // 摘要:
        //     An average counter designed to monitor the average length of a queue to a resource
        //     over time. It shows the difference between the queue lengths observed during
        //     the last two sample intervals divided by the duration of the interval. This type
        //     of counter is typically used to track the number of items that are queued or
        //     waiting.
        CountPerTimeInterval32 = 4523008,

        //
        // 摘要:
        //     An average counter that monitors the average length of a queue to a resource
        //     over time. Counters of this type display the difference between the queue lengths
        //     observed during the last two sample intervals, divided by the duration of the
        //     interval. This counter type is the same as CountPerTimeInterval32 except that
        //     it uses larger fields to accommodate larger values. This type of counter is typically
        //     used to track a high-volume or very large number of items that are queued or
        //     waiting.
        CountPerTimeInterval64 = 4523264,

        //
        // 摘要:
        //     A difference counter that shows the average number of operations completed during
        //     each second of the sample interval. Counters of this type measure time in ticks
        //     of the system clock.
        RateOfCountsPerSecond32 = 272696320,

        //
        // 摘要:
        //     A difference counter that shows the average number of operations completed during
        //     each second of the sample interval. Counters of this type measure time in ticks
        //     of the system clock. This counter type is the same as the RateOfCountsPerSecond32
        //     type, but it uses larger fields to accommodate larger values to track a high-volume
        //     number of items or operations per second, such as a byte-transmission rate.
        RateOfCountsPerSecond64 = 272696576,

        //
        // 摘要:
        //     An instantaneous percentage counter that shows the ratio of a subset to its set
        //     as a percentage. For example, it compares the number of bytes in use on a disk
        //     to the total number of bytes on the disk. Counters of this type display the current
        //     percentage only, not an average over time.
        RawFraction = 537003008,

        //
        // 摘要:
        //     A percentage counter that shows the average time that a component is active as
        //     a percentage of the total sample time.
        CounterTimer = 541132032,

        //
        // 摘要:
        //     A percentage counter that shows the active time of a component as a percentage
        //     of the total elapsed time of the sample interval. It measures time in units of
        //     100 nanoseconds (ns). Counters of this type are designed to measure the activity
        //     of one component at a time.
        Timer100Ns = 542180608,

        //
        // 摘要:
        //     A percentage counter that shows the average ratio of hits to all operations during
        //     the last two sample intervals.
        SampleFraction = 549585920,

        //
        // 摘要:
        //     A percentage counter that displays the average percentage of active time observed
        //     during sample interval. The value of these counters is calculated by monitoring
        //     the percentage of time that the service was inactive and then subtracting that
        //     value from 100 percent.
        CounterTimerInverse = 557909248,

        //
        // 摘要:
        //     A percentage counter that shows the average percentage of active time observed
        //     during the sample interval.
        Timer100NsInverse = 558957824,

        //
        // 摘要:
        //     A percentage counter that displays the active time of one or more components
        //     as a percentage of the total time of the sample interval. Because the numerator
        //     records the active time of components operating simultaneously, the resulting
        //     percentage can exceed 100 percent.
        CounterMultiTimer = 574686464,

        //
        // 摘要:
        //     A percentage counter that shows the active time of one or more components as
        //     a percentage of the total time of the sample interval. It measures time in 100
        //     nanosecond (ns) units.
        CounterMultiTimer100Ns = 575735040,

        //
        // 摘要:
        //     A percentage counter that shows the active time of one or more components as
        //     a percentage of the total time of the sample interval. It derives the active
        //     time by measuring the time that the components were not active and subtracting
        //     the result from 100 percent by the number of objects monitored.
        CounterMultiTimerInverse = 591463680,

        //
        // 摘要:
        //     A percentage counter that shows the active time of one or more components as
        //     a percentage of the total time of the sample interval. Counters of this type
        //     measure time in 100 nanosecond (ns) units. They derive the active time by measuring
        //     the time that the components were not active and subtracting the result from
        //     multiplying 100 percent by the number of objects monitored.
        CounterMultiTimer100NsInverse = 592512256,

        //
        // 摘要:
        //     An average counter that measures the time it takes, on average, to complete a
        //     process or operation. Counters of this type display a ratio of the total elapsed
        //     time of the sample interval to the number of processes or operations completed
        //     during that time. This counter type measures time in ticks of the system clock.
        AverageTimer32 = 805438464,

        //
        // 摘要:
        //     A difference timer that shows the total time between when the component or process
        //     started and the time when this value is calculated.
        ElapsedTime = 807666944,

        //
        // 摘要:
        //     An average counter that shows how many items are processed, on average, during
        //     an operation. Counters of this type display a ratio of the items processed to
        //     the number of operations completed. The ratio is calculated by comparing the
        //     number of items processed during the last interval to the number of operations
        //     completed during the last interval.
        AverageCount64 = 1073874176,

        //
        // 摘要:
        //     A base counter that stores the number of sampling interrupts taken and is used
        //     as a denominator in the sampling fraction. The sampling fraction is the number
        //     of samples that were 1 (or true) for a sample interrupt. Check that this value
        //     is greater than zero before using it as the denominator in a calculation of SampleFraction.
        SampleBase = 1073939457,

        //
        // 摘要:
        //     A base counter that is used in the calculation of time or count averages, such
        //     as AverageTimer32 and AverageCount64. Stores the denominator for calculating
        //     a counter to present "time per operation" or "count per operation".
        AverageBase = 1073939458,

        //
        // 摘要:
        //     A base counter that stores the denominator of a counter that presents a general
        //     arithmetic fraction. Check that this value is greater than zero before using
        //     it as the denominator in a RawFraction value calculation.
        RawBase = 1073939459,

        //
        // 摘要:
        //     A base counter that indicates the number of items sampled. It is used as the
        //     denominator in the calculations to get an average among the items sampled when
        //     taking timings of multiple, but similar items. Used with CounterMultiTimer, CounterMultiTimerInverse,
        //     CounterMultiTimer100Ns, and CounterMultiTimer100NsInverse.
        CounterMultiBase = 1107494144
    }

    public interface IPerformanceCounter
    {
        //
        // 摘要:
        //     Decrements the associated performance counter by one through an efficient atomic
        //     operation.
        //
        // 返回结果:
        //     The decremented counter value.
        //
        // 异常:
        //   T:System.InvalidOperationException:
        //     The counter is read-only, so the application cannot decrement it.-or- The instance
        //     is not correctly associated with a performance counter. -or-The System.Diagnostics.PerformanceCounter.InstanceLifetime
        //     property is set to System.Diagnostics.PerformanceCounterInstanceLifetime.Process
        //     when using global shared memory.
        //
        //   T:System.ComponentModel.Win32Exception:
        //     An error occurred when accessing a system API.
        //
        //   T:System.PlatformNotSupportedException:
        //     The platform is Windows 98 or Windows Millennium Edition (Me), which does not
        //     support performance counters.
        long Decrement();

        //
        // 摘要:
        //     Increments the associated performance counter by one through an efficient atomic
        //     operation.
        //
        // 返回结果:
        //     The incremented counter value.
        //
        // 异常:
        //   T:System.InvalidOperationException:
        //     The counter is read-only, so the application cannot increment it. -or- The instance
        //     is not correctly associated with a performance counter. -or- The System.Diagnostics.PerformanceCounter.InstanceLifetime
        //     property is set to System.Diagnostics.PerformanceCounterInstanceLifetime.Process
        //     when using global shared memory.
        //
        //   T:System.ComponentModel.Win32Exception:
        //     An error occurred when accessing a system API.
        //
        //   T:System.PlatformNotSupportedException:
        //     The platform is Windows 98 or Windows Millennium Edition (Me), which does not
        //     support performance counters.
        long Increment();

        //
        // 摘要:
        //     Increments or decrements the value of the associated performance counter by a
        //     specified amount through an efficient atomic operation.
        //
        // 参数:
        //   value:
        //     The value to increment by. (A negative value decrements the counter.)
        //
        // 返回结果:
        //     The new counter value.
        //
        // 异常:
        //   T:System.InvalidOperationException:
        //     The counter is read-only, so the application cannot increment it. -or- The instance
        //     is not correctly associated with a performance counter. -or- The System.Diagnostics.PerformanceCounter.InstanceLifetime
        //     property is set to System.Diagnostics.PerformanceCounterInstanceLifetime.Process
        //     when using global shared memory.
        //
        //   T:System.ComponentModel.Win32Exception:
        //     An error occurred when accessing a system API.
        //
        //   T:System.PlatformNotSupportedException:
        //     The platform is Windows 98 or Windows Millennium Edition (Me), which does not
        //     support performance counters.
        long IncrementBy(long value);

        //
        // 摘要:
        //     Obtains a counter sample and returns the calculated value for it.
        //
        // 返回结果:
        //     The next calculated value that the system obtains for this counter.
        //
        // 异常:
        //   T:System.InvalidOperationException:
        //     The instance is not correctly associated with a performance counter.
        //
        //   T:System.ComponentModel.Win32Exception:
        //     An error occurred when accessing a system API.
        //
        //   T:System.PlatformNotSupportedException:
        //     The platform is Windows 98 or Windows Millennium Edition (Me), which does not
        //     support performance counters.
        //
        //   T:System.UnauthorizedAccessException:
        //     Code that is executing without administrative privileges attempted to read a
        //     performance counter.
        float NextValue();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="value"></param>
        void SetRawValue(long value);
    }
}
