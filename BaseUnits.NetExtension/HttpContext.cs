using System;
using System.Net;

namespace BaseUnits.NetExtension
{
    /// <summary>
    /// 用户上下文对象
    /// </summary>
    public class HttpContext
    {
        public HttpListenerRequest Request { get; private set; }

        public HttpListenerResponse Response { get; private set; }

        public DateTime Created { get; private set; }

        internal HttpServer Server { get; private set; }

        internal HttpListenerContext Context { get; private set; }

        public long ContextId { get; private set; }

        public bool Complete { get; internal set; }

        public HttpContext(long contextId, HttpServer server, HttpListenerContext context)
        {
            ContextId = contextId;
            Created = DateTime.Now;

            Server = server;
            Context = context;

            Request = context.Request;
            Response = context.Response;
        }
    }
}
