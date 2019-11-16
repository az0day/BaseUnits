using BaseUnits.Core.Service;

namespace ZZTests.DestopApp.Modules
{
    internal abstract class BaseRemotingServer : BaseModule
    {
        public int ListenPort { get; }

        protected BaseRemotingServer(int listenPort)
        {
            ListenPort = listenPort;
        }




    }
}
