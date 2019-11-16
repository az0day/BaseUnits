using System.Threading;
using BaseUnits.Core.Service;
using BaseUnits.NetExtension;

namespace ZZTests.DestopApp.Modules
{
    class HttpApiModule : BaseHttpModule<ushort>
    {
        private readonly HttpServer _http;

        public HttpApiModule() : base(Config.Default.ListenHttpPort)
        {
            _http = new HttpServer(ListenPort);
        }

        #region .Required Methods.
        public override void Initialize()
        {
            _http.OnNoticeEvent += OnNoticeHttpEvent;
            _http.OnReceivedEvent += OnReceivedHttpEvent;

            Thread.Sleep(1000);
        }

        public override void Open()
        {
            _http.Start();

            Thread.Sleep(1000);
        }

        public override void Close()
        {
            _http.Stop();

            Thread.Sleep(1000);
        }

        protected override void Release()
        {
            _http.OnNoticeEvent -= OnNoticeHttpEvent;
            _http.OnReceivedEvent -= OnReceivedHttpEvent;

            _http.Dispose();

            Thread.Sleep(1000);
        }
        #endregion


        #region .HTTP Events Methods.

        private void OnNoticeHttpEvent(object sender, NoticeEventArgs e)
        {
            InvokeMessage(this, new MessageEventArgs(e.Title, e.Content));
        }

        private void OnReceivedHttpEvent(object sender, ReceivedEventArgs e)
        {
            ManageReceivedCounters();
            ManagePendingOperationsCounters();

            var ctx = e.Context;
            ctx.ReturnHtml("<h1>Fine, Thank you!</h1>");

            ManageCompletedOperationsCounters();
            ManageSentCounters();

        }


        #endregion
    }
}
