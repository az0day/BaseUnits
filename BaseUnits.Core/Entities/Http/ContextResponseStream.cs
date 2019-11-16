using System;
using System.IO;
using System.Net;

namespace BaseUnits.Core.Entities.Http
{
    /// <summary>
    /// ResponseStream
    /// </summary>
    [Serializable]
    public class ContextResponseStream : ContextBaseMessage
    {
        public ContextResponseStream(Uri uri, CookieContainer container)
        {
            Url = uri;
            Container = container;
        }

        public bool Error { get; set; }
        public string Message { get; set; }
        public Stream Stream { get; set; }

        public ContextResponseStream Success(Stream stream)
        {
            Stream = stream;
            Error = false;
            Message = "OK";
            return this;
        }

        public ContextResponseStream RedirectUrl(Uri uri)
        {
            Url = uri;
            return this;
        }

        public ContextResponseStream FoundError(string error)
        {
            Error = true;
            Message = error;
            return this;
        }
    }
}
