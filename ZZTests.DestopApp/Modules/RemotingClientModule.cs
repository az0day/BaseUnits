using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoreZeroTcp.Remoting;

namespace ZZTests.DestopApp.Modules
{
    class RemotingClientModule : BaseRemotingClient
    {
        private readonly TcpRemotingCaller _caller;

        public RemotingClientModule() : base(Config.Default.RemotingAddress, Config.Default.RemotingPort)
        {
            _caller = new TcpRemotingCaller(RemoteAddress, RemotePort);
        }


        #region .Required Methods.
        public override void Initialize()
        {

            Thread.Sleep(1000);
        }

        public override void Open()
        {
            _caller.Connect();

            Thread.Sleep(1000);
        }

        public override void Close()
        {
            _caller.Disconnect();

            Thread.Sleep(1000);
        }

        protected override void Release()
        {
            _caller.Dispose();

            Thread.Sleep(1000);
        }
        #endregion

        #region .TCP Events Methods.


        #endregion
    }
}
