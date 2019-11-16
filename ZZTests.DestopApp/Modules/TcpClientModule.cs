using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BaseUnits.Core.Service;
using BaseUnits.FormExtensions.Diagnostics;
using CoreZeroTcp.Abstracts;
using CoreZeroTcp.CommandBytes;
using static BaseUnits.Core.Helpers.LogsHelper;

namespace ZZTests.DestopApp.Modules
{
    class TcpClientModule : BaseTcpClient
    {
        public bool IsConnected => _tcpClient?.Connected ?? false;
        private readonly TcpByteClient _tcpClient;

        public TcpClientModule() : base(Config.Default.ServerAddress, Config.Default.ServerPort)
        {
            _tcpClient = new TcpByteClient(RemoteAddress, RemotePort);
        }

        #region .Required Methods.
        public override void Initialize()
        {
            var cid = new CorrelationId("ps/pcsclt/i/");

            _tcpClient.OnConnected += OnConnected;
            _tcpClient.OnDisconnected += OnDisconnected;
            _tcpClient.OnMessageReceived += OnReceived;

            try
            {
                ConnectionsCounter = new StatsCounterWin(StatsCountersFactory.TCP_SERVICE_CLIENT_CONNECTIONS);
                ReceivedPerSecondCounter = new StatsCounterWin(StatsCountersFactory.TCP_SERVICE_CLIENT_RECEIVED_PER_SECOND);
                ReceivedCounter = new StatsCounterWin(StatsCountersFactory.TCP_SERVICE_CLIENT_RECEIVED);
                SentPerSecondCounter = new StatsCounterWin(StatsCountersFactory.TCP_SERVICE_CLIENT_SENT_PER_SECOND);
                SentCounter = new StatsCounterWin(StatsCountersFactory.TCP_SERVICE_CLIENT_SENT);
            }
            catch (Exception ex)
            {
                LogException(cid, ex);
                throw;
            }

            Thread.Sleep(1000);
        }

        public override void Open()
        {
            _tcpClient.AutoReconnect = true;
            _tcpClient.Connect();

            Thread.Sleep(1000);
        }

        public override void Close()
        {
            _tcpClient.AutoReconnect = false;
            _tcpClient.Disconnect();

            Thread.Sleep(1000);
        }

        protected override void Release()
        {
            _tcpClient.AutoReconnect = false;

            _tcpClient.OnConnected -= OnConnected;
            _tcpClient.OnDisconnected -= OnDisconnected;
            _tcpClient.OnMessageReceived -= OnReceived;
            _tcpClient.Dispose();

            Thread.Sleep(1000);
        }
        #endregion

        #region .TCP Events Methods.

        private void OnReceived(IZeroContext context, int tid, byte[] bytes)
        {
            ManageReceivedCounters();

            try
            {
                ProcessReceivedMessage(tid, bytes);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private void OnConnected(object sender, RemoteContextArgs args)
        {
            LogTrace("Connections", $"[{Name}] Connected.", MessageLevelEnum.Both);

            ManageNewConnection();
        }

        private void OnDisconnected(object sender, RemoteContextArgs args)
        {
            LogFatal("Disconnected", MessageLevelEnum.MsgOnly);
            LogTrace("Connections", $"[{Name}] Disconnected.", MessageLevelEnum.Both);

            ManageDisconnection();
        }

        private void ProcessReceivedMessage(int tid, byte[] bytes)
        {
            ManageReceivedCounters();

            ManagePendingOperationsCounters();

            ManageCompletedOperationsCounters();

            ManageSentCounters();
        }

        #endregion
    }
}
