using System;
using System.Net;

namespace BaseUnits.Core.Entities.Http
{
    [Serializable]
    public class ContextResponse : ContextBaseMessage
    {
        public ContextResponse(Uri uri)
        {
            Url = uri;
        }

        public ContextResponse(Uri uri, CookieContainer container)
            : this(uri)
        {
            Container = container;
        }

        public bool Error { get; set; }
        public string Html = string.Empty;
        public string RedirectUrl = string.Empty;

        public ContextResponse Success(string html)
        {
            Html = html;
            Error = false;
            return this;
        }

        public ContextResponse FoundError(string error)
        {
            Error = true;
            Html = error;
            return this;
        }
    }
}
