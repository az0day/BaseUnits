using System;
using System.Collections.Concurrent;
using System.Threading;

namespace BaseUnits.Core.Helpers
{
    /// <summary>
    /// 单线程先进先出无锁执行队列
    /// </summary>
    public sealed class ThreadQueueAction : IDisposable
    {
        /// <summary>
        /// 用来存储生产者的队列
        /// </summary>
        private readonly ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();

        /// <summary>
        /// 出列的时候需要检查队列中是否有元素，如果没有，需要阻塞
        /// </summary>
        private readonly AutoResetEvent _queueWaitHandle = new AutoResetEvent(false);

        private readonly ManualResetEvent _disposingHandle = new ManualResetEvent(false);

        private bool _running;

        public int PendingsLimitMax = 0;

        private int _pendings;

        public int PendingCount
        {
            get
            {
                return _pendings;
            }
        }

        /// <summary>
        /// 执行失败时回调
        /// </summary>
        public event EventHandler OnCallbackFailed;

        public ThreadQueueAction()
        {
            _running = true;

            // 后台开启一个线程开始消费生产者
            new Thread(Consume)
            {
                IsBackground = true,
                Priority = ThreadPriority.Lowest
            }.Start();
        }

        /// <summary>
        /// New Item
        /// </summary>
        /// <param name="item"></param>
        public bool Push(Action item)
        {
            if (_disposed)
            {
                return false;
            }

            if (PendingsLimitMax > 0 && _pendings > PendingsLimitMax)
            {
                return false;
            }

            _queue.Enqueue(item);
            Interlocked.Increment(ref _pendings);

            _queueWaitHandle.Set();

            return true;
        }

        /// <summary>
        /// Consume
        /// </summary>
        private void Consume()
        {
            var waitHandlers = new WaitHandle[]
            {
                // 执行阻塞
                _queueWaitHandle,

                // 资源释放标识
                _disposingHandle
            };

            while (_running)
            {
                var ret = WaitHandle.WaitAny(waitHandlers);
                if (ret == 0)
                {
                    while (_queue.TryDequeue(out var nextItem))
                    {
                        try
                        {
                            nextItem();
                        }
                        catch (Exception ex)
                        {
                            LogsHelper.Error("Queue-Consumer", ex.ToString());
                            OnCallbackFailed?.Invoke(this, new ActionCallbackFailedEventArgs(ex));
                        }
                        finally
                        {
                            Interlocked.Decrement(ref _pendings);
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private bool _disposed;

        /// <summary>
        /// https://msdn.microsoft.com/zh-cn/library/fs2xkftw(v=vs.110).aspx
        /// </summary>
        public void Dispose()
        {
            // 显式释放
            // 必须为true
            Dispose(true);

            // 通知垃圾回收机制不再调用终结器（析构器）
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// for child class
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // 清理使用的资源
                // 设置状态
                // 释放资源
                _disposingHandle.Set();
                _running = false;

                Thread.Yield();
            }

            // 标识为已释放
            _disposed = true;
        }

        /// <summary>
        /// 必须，以备程序员忘记了显式调用Dispose方法
        /// </summary>
        ~ThreadQueueAction()
        {
            Dispose(false);
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public class ActionCallbackFailedEventArgs : EventArgs
        {
            public readonly Exception Exception;

            public ActionCallbackFailedEventArgs(Exception exception)
            {
                Exception = exception;
            }
        }
    }
}
