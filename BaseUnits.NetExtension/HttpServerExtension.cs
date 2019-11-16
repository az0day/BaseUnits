using System.Text;
using BaseUnits.Core.Helpers;

namespace BaseUnits.NetExtension
{
    public static class HttpContextExtension
    {
        /// <summary>
        /// 已完成
        /// </summary>
        /// <param name="context"></param>
        private static void Complete(this HttpContext context)
        {
            context.Server.Complete(context.ContextId);
        }

        public static void ReturnText(this HttpContext context, string text)
        {
            context.Context.ReturnText(text);
            context.Complete();
        }

        public static void ReturnJson<T>(this HttpContext context, T data)
        {
            var json = StaticHelper.ToJson(data);
            context.Context.ReturnJson(json);
            context.Complete();
        }

        public static void ReturnXml(this HttpContext context, string text)
        {
            context.Context.ReturnXml(text);
            context.Complete();
        }

        public static void ReturnHtml(this HttpContext context, string text)
        {
            context.Context.ReturnHtml(text);
            context.Complete();
        }

        public static void ReturnContent(this HttpContext context, string text, string contentType, string encoding = "UTF-8")
        {
            context.Context.ReturnContent(text, contentType, Encoding.GetEncoding(encoding));
            context.Complete();
        }

        /// <summary>
        /// 204 - NoContent
        /// </summary>
        /// <param name="context"></param>
        public static void NoContent(this HttpContext context)
        {
            context.Context.NoContent();
            context.Complete();
        }

        /// <summary>
        /// 403 - Forbidden
        /// </summary>
        /// <param name="context"></param>
        public static void Forbidden(this HttpContext context)
        {
            context.Context.Forbidden();
            context.Complete();
        }

        /// <summary>
        /// 404 - NotFound
        /// </summary>
        /// <param name="context"></param>
        public static void NotFound(this HttpContext context)
        {
            context.Context.NotFound();
            context.Complete();
        }

        /// <summary>
        /// 503 - Service Unavailable
        /// </summary>
        /// <param name="context"></param>
        public static void ServiceUnavailable(this HttpContext context)
        {
            context.Context.ServiceUnavailable();
            context.Complete();
        }
    }
}
