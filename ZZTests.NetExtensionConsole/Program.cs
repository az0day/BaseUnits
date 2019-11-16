using BaseUnits.NetExtension;
using System;

namespace ZZTests.NetExtensionConsole
{
    class Program
    {
        static void Main()
        {
            HttpServer.DebugContext = true;

            var httpServer = new HttpServer(Config.Default.ListenPort);

            httpServer.OnReceivedEvent += HttpServer_OnReceivedEvent;
            httpServer.OnNoticeEvent += HttpServer_OnNoticeEvent;

            httpServer.Start();

            Console.WriteLine("Press any key to stop http server ...");
            Console.ReadLine();

            httpServer.Stop();
            httpServer.Dispose();
        }

        private static void HttpServer_OnReceivedEvent(object sender, EventArgs e)
        {
            var args = (ReceivedEventArgs) e;
            var ctx = args.Context;
            var business = new BusinessLogic(ctx);

            var request = ctx.Request;
            var url = request.Url;
            var host = url.Host;

            if (business.Process())
            {
                return;
            }
            
            var html = $@"
<html>
<body>
    <h1>Hello World!</h1>
    <p>Url: {url}</p>
    <p>Host: {host}</p>
    <p>PathAndQuery: {url.PathAndQuery}</p>
    <p>LocalPath: {url.LocalPath}</p>
    <p>AbsolutePath: {url.AbsolutePath}</p>
    <p>AbsoluteUri: {url.AbsoluteUri}</p>
    <p>IsDefaultPort: {url.IsDefaultPort}</p>
    <pre>
    //public unsafe void SetRequestQueueLength(HttpListener listener, long len)
    //{{
        //    var requestQueueHandle = (CriticalHandle)requestQueueHandleProperty.GetValue(listener, null);
        //    var result = HttpSetRequestQueueProperty(requestQueueHandle, HttpServerProperty.HttpServerQueueLengthProperty, new IntPtr((void*)&len), (uint)Marshal.SizeOf(len), 0, IntPtr.Zero);

        //    if (result != 0)
        //    {{
        //        var ex = new HttpListenerException((int)result);
        //        ShowMessage(ex);
        //        LogsError(ex);
        //    }}
    //}}

    //internal enum HttpServerProperty
    //{{
    //    HttpServerAuthenticationProperty,
    //    HttpServerLoggingProperty,
    //    HttpServerQosProperty,
    //    HttpServerTimeoutsProperty,
    //    HttpServerQueueLengthProperty,
    //    HttpServerStateProperty,
    //    HttpServer503VerbosityProperty,
    //    HttpServerBindingProperty,
    //    HttpServerExtendedAuthenticationProperty,
    //    HttpServerListenEndpointProperty,
    //    HttpServerChannelBindProperty,
    //    HttpServerProtectionLevelProperty,
    //}}

    //internal static extern uint HttpSetRequestQueueProperty(
    //    CriticalHandle requestQueueHandle,
    //    HttpServerProperty serverProperty,
    //    IntPtr pPropertyInfo,
    //    uint propertyInfoLength,
    //    uint reserved,
    //    IntPtr pReserved
    //);
    </pre>
</body>
</html>
            ";

            ctx.ReturnHtml(html);
        }

        private static void HttpServer_OnNoticeEvent(object sender, EventArgs e)
        {
            var args = (NoticeEventArgs) e;
            Console.WriteLine("[{0}] - {1}", args.Title, args.Content);
        }
    }
}
