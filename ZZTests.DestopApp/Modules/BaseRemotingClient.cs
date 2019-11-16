using BaseUnits.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZZTests.DestopApp.Modules
{
    abstract class BaseRemotingClient : BaseModule
    {
        public string RemoteAddress { get; }
        public int RemotePort { get; }

        protected BaseRemotingClient(string remoteAddress, int remotePort)
        {
            RemoteAddress = remoteAddress;
            RemotePort = remotePort;
        }
    }
}
