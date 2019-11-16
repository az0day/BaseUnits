using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using BaseUnits.Core.Helpers;

namespace BaseUnits.Remotings
{
    public sealed class RemotingClientPool : IDisposable
    {
        private static long clientId;

        /// <summary>
        /// Pool
        /// </summary>
        private readonly ConcurrentStack<Connector> _pool;

        private readonly ConcurrentDictionary<long, Connector> _connectings = new ConcurrentDictionary<long, Connector>();
        private int _currents;

        /// <summary>
        /// Semaphore
        /// </summary>
        private readonly Semaphore _sema;

        /// <summary>
        /// TIME OUT
        /// </summary>
        private const int TIME_OUT = 20000;

        /// <summary>
        /// 远程地址
        /// </summary>
        private readonly string _remoteHost;

        /// <summary>
        /// 远程地址
        /// </summary>
        private readonly int _remotePort;

        /// <summary>
        /// 最大连接
        /// </summary>
        private readonly int _maxConnections = 5;

        /// <summary>
        /// 是否已初始化
        /// </summary>
        private bool _inited;

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="capacity"></param>
        public RemotingClientPool(string address, int port, int capacity = 10)
        {
            _maxConnections = capacity;
            _remoteHost = address;
            _remotePort = port;

            _sema = new Semaphore(0, capacity);
            _pool = new ConcurrentStack<Connector>();

            CreateOneConnector();
        }



        /// <summary>
        /// CreateFactory
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        private Connector CreateInstance(string address, int port)
        {
            var id = Interlocked.Increment(ref clientId);
            return new Connector(id, address, port);
        }

        /// <summary>
        /// GetFactory
        /// </summary>
        /// <returns></returns>
        private Connector GetOne()
        {
            if (!_sema.WaitOne(TIME_OUT))
            {
                return null;
            }

            // 拿出可用连接
            while (_pool.TryPop(out var one))
            {
                if (one.Connected)
                {
                    // 已连接直接返回
                    return one;
                }

                // 放入正在连接池
                _connectings[one.Id] = one;
            }

            // 创建更多连接
            while (_currents < _maxConnections)
            {
                CreateOneConnector();

            }

            return null;
        }

        /// <summary>
        /// 压缩执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public T Execute<T>(int command, params object[] args)
        {
            var client = GetOne();

            if (client != null)
            {
                try
                {
                    var bytes = BinaryHelper.ToBytes(args, true);
                    var response = client.Request(bytes);

                    if (response != null && response.Length > 0)
                    {
                        var data = BinaryHelper.FromBytes<T>(response);
                        return data;
                    }

                    return default(T);
                }
                catch (Exception ex)
                {
                    var error = string.Format(
                        "public T Execute<T>(int command, params object[] args){0}uri: {1}:{2} - {3}{0}Error: {4}",
                        Environment.NewLine,
                        _remoteHost,
                        _remotePort,
                        StaticHelper.ToJson(args),
                        ex
                    );
                    LogsHelper.Error("Remoting.ExecuteT", error);
                    throw;
                }
                finally
                {
                    Push(client);
                }
            }

            return default(T);
        }

        /// <summary>
        /// 重新放入
        /// </summary>
        /// <param name="connector"></param>
        private void Push(Connector connector)
        {
            _pool.Push(connector);
            _sema.Release();
        }


        /// <summary>
        /// 初始化
        /// </summary>
        private void CreateOneConnector()
        {
            Interlocked.Increment(ref _currents);

            var connector = CreateInstance(_remoteHost, _remotePort);

            // 放入待连接列表
            _connectings[connector.Id] = connector;

            connector.OnConnectEvent += Connector_OnConnectEvent;
            connector.OnDisconnectedEvent += Connector_OnDisconnectedEvent;

            connector.Connect();
        }

        private void Connector_OnDisconnectedEvent(object sender, EventArgs e)
        {
            var connector = (Connector)sender;
            connector.Connect();
        }

        private void Connector_OnConnectEvent(object sender, EventArgs e)
        {
            var connector = (Connector) sender;
            var id = connector.Id;

            // 从正在连接的列表中移除
            _connectings.TryRemove(id, out _);

            // 放入连接池
            Push(connector);
        }

        /// <summary>
        /// 发送（参数未压缩）
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        public void Post(int command, params object[] args)
        {
            var client = GetOne();

            try
            {
                var bytes = BinaryHelper.ToBytes(args, true);
                client.Post(bytes);
            }
            catch (Exception ex)
            {
                var error = string.Format(
                    "public void Post(int command, params object[] args){0}uri: {1}:{2} - {3}{0}Error: {4}",
                    Environment.NewLine,
                    _remoteHost,
                    _remotePort,
                    StaticHelper.ToJson(args),
                    ex
                );
                LogsHelper.Error("Remoting.Post", error);
                throw;
            }
            finally
            {
                Push(client);
            }
        }

        public void Dispose()
        {
            _sema.Dispose();
        }
    }
}
