using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoreZeroTcp.Abstracts;
using CoreZeroTcp.Remoting;
using ZZTests.DestopApp.Cache;

namespace ZZTests.DestopApp.Modules
{
    internal class RemotingServerModule : BaseRemotingServer
    {
               
        #region Properties and Members
        private Trunk _trunk;

        private OnlineUsersCache _onlineUsersCache;
        private TcpRemotingServer _server;


        #endregion

        public RemotingServerModule() : base(Config.Default.ListenRemotingPort)
        {
            _server = new TcpRemotingServer();
        }


        #region .Required Methods.
        public override void Initialize()
        {
            _trunk = Trunk.Instance;
            _onlineUsersCache = OnlineUsersCache.Instance;

            _server.OnReceivedClientMessage += HandleOnMessageReceived;


            Thread.Sleep(1000);
        }

        public override void Open()
        {
            _server.Start(ListenPort);

            Thread.Sleep(1000);
        }

        public override void Close()
        {
            _server.ShutDown();

            Thread.Sleep(1000);
        }

        protected override void Release()
        {
            _server.OnReceivedClientMessage -= HandleOnMessageReceived;
            _server.Dispose();

            Thread.Sleep(1000);

        }
        #endregion

        #region .TCP Events Methods.
        private void HandleOnMessageReceived(object sender, RemotingEventArgs args)
        {
            ManageReceivedCounters();
        }
        #endregion
    }
}
