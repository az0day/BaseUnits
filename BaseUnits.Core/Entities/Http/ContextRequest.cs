using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BaseUnits.Core.Entities.Http
{
    [Serializable]
    public class ContextRequest : ContextBaseMessage
    {
        public ContextRequest(string uri)
        {
            Url = new Uri(uri);
            Container = new CookieContainer();
        }

        public string Method = "GET";
        public string PostData = "";
        public string RefererUrl = "";
        public string Accept = "";

        public string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";
        public  Encoding RequestEncoding = Encoding.UTF8;

        public bool UpdateCookies { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}
