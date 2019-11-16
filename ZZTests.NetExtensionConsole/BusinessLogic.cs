using System;
using System.Collections.Generic;
using BaseUnits.NetExtension;

namespace ZZTests.NetExtensionConsole
{
    public class BusinessLogic
    {
        private readonly HttpContext _context;

        public BusinessLogic(HttpContext context)
        {
            _context = context;
        }

        private static readonly IDictionary<string, Action<HttpContext>> _actions = new Dictionary<string, Action<HttpContext>>
            {
                {"/hello", Hello},

                {"/api", ApiContext},
                {"/api/html", ApiHtml},
                {"/api/json/object", ApiJsonObject},
                {"/api/json/text", ApiJsonText},
                {"/api/xml", ApiXml},
                {"/api/text", ApiText},
                {"/api/css", ApiCss},

                {"/status/503", ServiceUnavailable},
                {"/status/204", NoContent},
                {"/status/404", NotFound},
                {"/status/403", Forbidden}
            };

        public bool Process()
        {
            var request = _context.Request;
            var url = request.Url;
            var host = url.Host;
            var apiUrl = url.AbsolutePath.TrimEnd('/');

            Action<HttpContext> action;
            if (_actions.TryGetValue(apiUrl, out action))
            {
                action(_context);
                return true;
            }

            return false;
        }

        private static void Hello(HttpContext context)
        {
            var request = context.Request;
            var name = request.QueryString["name"] ?? "";

            context.ReturnHtml($"<h1>Hello {name}</h1>");
        }

        private static void ApiContext(HttpContext context)
        {
            context.ReturnHtml("/api");
        }

        private static void ApiHtml(HttpContext context)
        {
            context.ReturnHtml("html");
        }

        private static void ApiJsonObject(HttpContext context)
        {
            var array = _actions.Keys;
            context.ReturnJson(array);
        }

        private static void ApiJsonText(HttpContext context)
        {
            context.ReturnJson(@"{""ok"": true}");
        }

        private static void ApiXml(HttpContext context)
        {
            context.ReturnXml(@"<xml><root><item key=""1"" value=""100"" /><item key=""2"" value=""200"" /></root></xml>");
        }

        private static void ApiText(HttpContext context)
        {
            context.ReturnText("Text");
        }

        private static void ApiCss(HttpContext context)
        {
            context.ReturnContent("body{}", "text/css; charset=utf-8");
        }

        private static void ServiceUnavailable(HttpContext context)
        {
            context.ServiceUnavailable();
        }

        private static void NoContent(HttpContext context)
        {
            context.NoContent();
        }

        private static void Forbidden(HttpContext context)
        {
            context.Forbidden();
        }

        private static void NotFound(HttpContext context)
        {
            context.NotFound();
        }
    }
}
