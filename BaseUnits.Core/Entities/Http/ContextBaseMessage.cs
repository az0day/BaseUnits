using System;
using System.Net;

namespace BaseUnits.Core.Entities.Http
{
    [Serializable]
    public abstract class ContextBaseMessage
    {
        /// <summary>
        /// Request Url
        /// </summary>
        public Uri Url { get; protected set; }

        /// <summary>
        /// Container
        /// </summary>
        public CookieContainer Container { get; protected set; }
    }
}
