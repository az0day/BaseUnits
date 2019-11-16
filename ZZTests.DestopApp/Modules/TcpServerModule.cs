using CoreZeroTcp.Abstracts;
using CoreZeroTcp.CommandBytes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZZTests.DestopApp.Cache;

namespace ZZTests.DestopApp.Modules
{
    class TcpServerModule : BaseTcpServer
    {
        #region .Properties and Members.
        private OnlineUsersCache _onlineUsersCache;
        private Trunk _trunk;

        private TcpByteServer _server;
        #endregion

        public TcpServerModule() : base(Config.Default.ListenPort)
        {
            _server = new TcpByteServer();
        }


        #region .Required Methods.
        public override void Initialize()
        {
            _trunk = Trunk.Instance;
            _onlineUsersCache = OnlineUsersCache.Instance;

            _server.OnMessageReceived += HandleOnMessageReceived;
            _server.OnConnected += HandleOnClientConnected;
            _server.OnDisconnected += HandleOnClientDisconnected;


            Thread.Sleep(1000);
        }

        public override void Open()
        {

            Thread.Sleep(1000);

        }

        public override void Close()
        {
            Thread.Sleep(1000);

        }

        protected override void Release()
        {

            Thread.Sleep(1000);
        }
        #endregion


        #region .TCP Events Methods.

        private void HandleOnClientDisconnected(object sender, RemoteContextArgs args)
        {
            ManageDisconnection();
        }

        private void HandleOnClientConnected(object sender, RemoteContextArgs args)
        {
            ManageNewConnection();
        }

        private void HandleOnMessageReceived(IZeroContext context, int tid, byte[] data)
        {
            ManageReceivedCounters();
        }
        #endregion

    }
}
