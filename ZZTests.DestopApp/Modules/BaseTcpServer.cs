using BaseUnits.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZZTests.DestopApp.Modules
{
    abstract class BaseTcpServer : BaseModule
    {
        public int ListenPort { get; }

        protected BaseTcpServer(int listenPort)
        {
            ListenPort = listenPort;
        }


    }
}
