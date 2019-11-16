using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using BaseUnits.NetExtension.Inner;

namespace BaseUnits.NetExtension
{
    public class HttpServer : IDisposable
    {
        /// <summary>
        /// 全局标识
        /// </summary>
        private static int _serverIndex;

        public static bool DebugContext;

        /// <summary>
        /// 当前实体标识
        /// </summary>
        private readonly int _bindIndex;

        private readonly ushort _listenPort;
        private readonly string _bindIp;
        private readonly string _bindPrefix;

        /// <summary>
        /// 标识
        /// </summary>
        public string Title { get; protected set; }

        private bool _stopped;
        private int _pendings;
        private long _index;

        /// <summary>
        /// 实例标识
        /// </summary>
        public int Index => _bindIndex;

        public string BindIp => _bindIp;

        public int ListenPort => _listenPort;

        public string Prefixes => _bindPrefix;

        public int Pendings => _pendings;

        /// <summary>
        /// > 0: 限制处理队列
        /// </summary>
        public int QueueMax { get; set; } = 65536;

        /// <summary>
        /// > 0: 限制并发队列
        /// </summary>
        public int ConcurrentMax { get; set; } = 10000;

        private readonly HttpListener _httpListener;

        /// <summary>
        /// Request Timeout
        /// </summary>
        public int RequestTimeout { get; set; } = 120000;

        public event EventHandler<ReceivedEventArgs> OnReceivedEvent;
        public event EventHandler<NoticeEventArgs> OnNoticeEvent;
        private Thread _thread;
        private AutoResetEvent _contextEvent = new AutoResetEvent(false);

        public HttpServer(ushort bindPort = 80) : this(bindPort, "")
        {

        }

        public HttpServer(ushort bindPort, string bindIp)
        {
            ServicePointManager.UseNagleAlgorithm = true;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.CheckCertificateRevocationList = true;
            ServicePointManager.DefaultConnectionLimit = 65535;

            // 设置全局标识
            _bindIndex = Interlocked.Increment(ref _serverIndex);

            var type = GetType().ToString().Split('.');
            var className = type[type.Length - 1];
            Title = $"{className}[{_bindIndex}]";

            _listenPort = bindPort;
            _bindIp = bindIp;

            // 设置
            if (string.IsNullOrEmpty(_bindIp))
            {
                // 使用 + 号可以不用管理员权限
                _bindIp = "+";
            }

            _bindPrefix = $"http://{_bindIp}:{bindPort}/";

            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(_bindPrefix);
            //httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            //_httpListener.IgnoreWriteExceptions = false;
            _httpListener.IgnoreWriteExceptions = false;
            //httpListener.UnsafeConnectionNtlmAuthentication = true;

            // 设置等待队列的最大长度
            // set queue length
            // SetRequestQueueLength(_httpListener, 10000);
        }

        public void Start()
        {
            if (!HttpListener.IsSupported)
            {
                ShowMessage("Windows XP SP2 or Server 2003 or upper version is required to use the HttpListener class.");
                return;
            }

            if (!_httpListener.IsListening)
            {
                //SetRequestQueueLength(_httpListener, 100000);
                try
                {
                    _httpListener.Start();
                    _httpListener.BeginGetContext(GetContextCallBack, _httpListener);

                    _stopped = false;

                    // 启动清理线程
                    _thread = new Thread(new ParameterizedThreadStart(HandlePendingContexts));
                    _thread.Start();

                    ShowMessage($"{_bindPrefix} listen successfully.");
                }
                catch (Exception ex)
                {
                    Logger.Error(Title, string.Format("HttpService listen failed. (port: {0}) Error: {1}", _listenPort, ex));
                    throw;
                }
            }
        }

        private void HandlePendingContexts(object state)
        {
            while (!_stopped)
            {
                // 最短处理时间
                var timeout = Math.Max(RequestTimeout, 30000);
                if (_contextEvent.WaitOne(timeout))
                {
                    var before = DateTime.Now.AddMilliseconds(-timeout);
                    var query = (
                        from p in _contexts.Values
                        where p.Created < before
                        select p.ContextId
                    ).ToArray();

                    foreach (var id in query)
                    {
                        ReleaseContext(id);
                    }
                }
            }
        }

        /// <summary>
        /// Stop Http Listener
        /// </summary>
        public void Stop()
        {
            try
            {
                _stopped = true;

                _httpListener.Stop();
                _httpListener.Close();
            }
            catch (Exception ex)
            {
                ShowMessage(ex);
                LogsError(ex);
            }
        }

        #region .Trace & Logs.
        private void ShowMessage(string title, string content)
        {
            OnNoticeEvent?.Invoke(this, new NoticeEventArgs(title, content));
        }

        private void ShowMessage(string content)
        {
            ShowMessage(Title, content);
        }

        private void ShowMessage(Exception ex)
        {
            ShowMessage(Title, ex.ToString());
        }

        private void LogsError(Exception ex)
        {
            Logger.Error(Title, ex);
        }
        #endregion

        //public unsafe void SetRequestQueueLength(HttpListener listener, long len)
        //{
        //    var listenerType = typeof(HttpListener);
        //    var requestQueueHandleProperty = listenerType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).First(p => p.Name == "RequestQueueHandle");

        //    var requestQueueHandle = (CriticalHandle)requestQueueHandleProperty.GetValue(listener, null);
        //    var result = HttpSetRequestQueueProperty(requestQueueHandle, HttpServerProperty.HttpServerQueueLengthProperty, new IntPtr((void*)&len), (uint)Marshal.SizeOf(len), 0, IntPtr.Zero);

        //    if (result != 0)
        //    {
        //        var ex = new HttpListenerException((int)result);
        //        ShowMessage(ex);
        //        LogsError(ex);
        //    }
        //}

        //internal enum HttpServerProperty
        //{
        //    HttpServerAuthenticationProperty,
        //    HttpServerLoggingProperty,
        //    HttpServerQosProperty,
        //    HttpServerTimeoutsProperty,
        //    HttpServerQueueLengthProperty,
        //    HttpServerStateProperty,
        //    HttpServer503VerbosityProperty,
        //    HttpServerBindingProperty,
        //    HttpServerExtendedAuthenticationProperty,
        //    HttpServerListenEndpointProperty,
        //    HttpServerChannelBindProperty,
        //    HttpServerProtectionLevelProperty,
        //}

        //[DllImport("httpapi.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        //internal static extern uint HttpSetRequestQueueProperty(
        //    CriticalHandle requestQueueHandle,
        //    HttpServerProperty serverProperty,
        //    IntPtr pPropertyInfo,
        //    uint propertyInfoLength,
        //    uint reserved,
        //    IntPtr pReserved
        //);

        private void GetContextCallBack(IAsyncResult result)
        {
            var listener = (HttpListener)result.AsyncState;
            if (!listener.IsListening)
            {
                if (_stopped)
                {
                    Logger.Trace(Title, "Listener Stopped.");
                    return;
                }

                Logger.Error(Title, $"stopped - {_stopped}");
                return;
            }

            HttpListenerContext context = null;
            try
            {
                context = listener.EndGetContext(result);
            }
            catch (Exception ex)
            {
                ShowMessage(ex);
                LogsError(ex);

                if (context != null)
                {
                    try
                    {
                        context.Response.Abort();
                        context.Response.Close();
                    }
                    // ReSharper disable EmptyGeneralCatchClause
                    catch { }
                    // ReSharper restore EmptyGeneralCatchClause
                }
            }
            finally
            {
                listener.BeginGetContext(GetContextCallBack, listener);
            }

            // Handle
            HandleRequest(context);
        }

        private void HandleRequest(HttpListenerContext context)
        {
            // 检查当前等待队列
            if (context != null)
            {
                // 待处理的队列数量
                if (QueueMax > 0 && _pendings >= QueueMax)
                {
                    SystemBusyHandler(context);
                    return;
                }

                // 当前正在处理且未正常释放的对象数量
                if (ConcurrentMax > 0 && _contexts.Count >= ConcurrentMax)
                {
                    SystemBusyHandler(context);
                    return;
                }

                var handler = OnReceivedEvent;
                if (handler == null)
                {
                    // 用户未处理功能
                    context.ServiceNotImplemented();
                    return;
                }

                // 等待回调队列
                Interlocked.Increment(ref _pendings);
                var contextId = Interlocked.Increment(ref _index);

                // 创建上下文
                var ctx = new HttpContext(contextId, this, context);
                MonitorContext(ctx);

                ThreadPool.QueueUserWorkItem(delegate
                {
                    try
                    {
                        handler.Invoke(this, new ReceivedEventArgs(ctx));
                    }
                    catch (Exception ex)
                    {
                        LogsError(ex);
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _pendings);
                    }
                });
            }
            else
            {
                LogsError(new Exception("context is null."));
            }
        }

        #region .Monitor & Clean up.
        private readonly ConcurrentDictionary<long, HttpContext> _contexts = new ConcurrentDictionary<long, HttpContext>();

        private void MonitorContext(HttpContext context)
        {
            var success = _contexts.TryAdd(context.ContextId, context);
            if (success)
            {
                _contextEvent.Set();
            }

            // 清理超时队列
        }

        internal void ReleaseContext(long contextId)
        {
            HttpContext context;
            if (!_contexts.TryRemove(contextId, out context))
            {
                return;
            }

            // 监控日志
            if (DebugContext)
            {
                var request = context.Request;
                var url = request.Url;
                ShowMessage($"released. url: {url}");
            }

            if (!context.Complete)
            {
                // 408 Request Timeout
                var ctx = context.Context;
                ctx.Status(408);
            }

            // 已完成
            context.Complete = true;
        }

        internal void Complete(long contextId)
        {
            HttpContext context;
            if (!_contexts.TryGetValue(contextId, out context))
            {
                return;
            }

            // 已完成
            context.Complete = true;
        }

        private void SystemBusyHandler(HttpListenerContext context)
        {
            context.ServiceUnavailable();
        }
        #endregion

        private bool _disposed;

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
                // 释放资源


                Thread.Yield();
            }

            // 标识为已释放
            _disposed = true;
        }

        /// <summary>
        /// 必须，以备程序员忘记了显式调用Dispose方法
        /// </summary>
        ~HttpServer()
        {
            Dispose(false);
        }
    }
}
