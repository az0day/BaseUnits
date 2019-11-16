using System.Threading;
using BaseUnits.Core.Diagnostics;
using BaseUnits.Core.Helpers;

namespace BaseUnits.Core.Service
{
    /// <summary>
    /// 带统计的模块
    /// </summary>
    public abstract class BaseServiceCounter : BaseService
    {
        public virtual IStatsCounter ConnectionsCounter { get; protected set; }
        public virtual IStatsCounter ReceivedPerSecondCounter { get; protected set; }
        public virtual IStatsCounter ReceivedCounter { get; protected set; }
        public virtual IStatsCounter SentPerSecondCounter { get; protected set; }
        public virtual IStatsCounter SentCounter { get; protected set; }
        public virtual IStatsCounter PendingOperationsCounter { get; protected set; }
        public virtual IStatsCounter OperationsPerSecondCounter { get; protected set; }
        public virtual IStatsCounter CompletedOperationsCounter { get; protected set; }

        public virtual long Connections => ConnectionsCounter?.NextValue ?? ConnectionsInternal;

        public virtual long ReceivedPerSecond => ReceivedPerSecondCounter?.NextValue ?? 0;
        public virtual long Received => ReceivedCounter?.NextValue ?? ReceivedInternal;

        public virtual long SentPerSecond => SentPerSecondCounter?.NextValue ?? 0;
        public virtual long Sent => SentCounter?.NextValue ?? SentInternal;

        public virtual long PendingOperations => PendingOperationsCounter?.NextValue ?? PendingOperationsInternal;
        public virtual long OperationsPerSecond => OperationsPerSecondCounter?.NextValue ?? 0;
        public virtual long CompletedOperations => CompletedOperationsCounter?.NextValue ?? CompletedOperationsInternal;

        protected long ConnectionsInternal;
        protected long ReceivedInternal;
        protected long SentInternal;
        protected long PendingOperationsInternal;
        protected long CompletedOperationsInternal;

        #region Protected Methods
        protected virtual void ManageNewConnection(int value = 1)
        {
            Interlocked.Add(ref ConnectionsInternal, value);
            ConnectionsCounter?.Increment(value);
        }

        protected virtual void ManageDisconnection(int value = 1)
        {
            Interlocked.Add(ref ConnectionsInternal, -value);
            ConnectionsCounter?.Increment(-value);
        }

        protected virtual void ManageReceivedCounters(int value = 1)
        {
            Interlocked.Add(ref ReceivedInternal, value);
            ReceivedCounter?.Increment(value);
            ReceivedPerSecondCounter?.Increment(value);
        }
        
        protected virtual void ManageSentCounters(int value = 1)
        {
            Interlocked.Add(ref SentInternal, value);
            SentCounter?.Increment(value);
            SentPerSecondCounter?.Increment(value);
        }

        protected virtual void ManagePendingOperationsCounters()
        {
            Interlocked.Increment(ref PendingOperationsInternal);
            PendingOperationsCounter?.Increment();
        }

        protected virtual void ManageCompletedOperationsCounters()
        {
            Interlocked.Decrement(ref PendingOperationsInternal);
            PendingOperationsCounter?.Decrement();

            Interlocked.Increment(ref CompletedOperationsInternal);
            CompletedOperationsCounter?.Increment();
            OperationsPerSecondCounter?.Increment();
        }
        #endregion
    }
}
