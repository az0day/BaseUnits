using BaseUnits.Core.Service;

namespace ZZTests.DestopApp.Modules
{
    abstract class BaseHttpModule<TPort> : BaseModule
    {
        public TPort ListenPort { get; private set; }

        protected BaseHttpModule(TPort listenPort)
        {
            ListenPort = listenPort;
        }
}
}
