using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using BaseUnits.NetExtension.Inner;

namespace BaseUnits.NetExtension
{
    internal static class HttpServerInternalExtension
    {
        private const int STATUS_OK = 200;
        private const string CONTENT_APPLICATION_JSON = "application/json; charset=utf-8";
        private const string CONTENT_APPLICATION_XML = "application/xml; charset=utf-8";
        private const string CONTENT_TEXT_HTML = "text/html; charset=utf-8";
        private const string CONTENT_TEXT_PLAIN = "text/plain; charset=utf-8";
        //private const string CONTENT_TEXT_XML = "text/xml; charset=utf-8";
        
        public static void ReturnJson(this HttpListenerContext context, string text)
        {
            var gzip = ParseGZipOptions(context);
            context.Content(STATUS_OK, text, CONTENT_APPLICATION_JSON, Encoding.UTF8, gzip);
        }

        public static void ReturnXml(this HttpListenerContext context, string text)
        {
            var gzip = ParseGZipOptions(context);
            context.Content(STATUS_OK, text, CONTENT_APPLICATION_XML, Encoding.UTF8, gzip);
        }

        public static void ReturnText(this HttpListenerContext context, string text)
        {
            var gzip = ParseGZipOptions(context);
            context.Content(STATUS_OK, text, CONTENT_TEXT_PLAIN, Encoding.UTF8, gzip);
        }

        public static void ReturnHtml(this HttpListenerContext context, string text)
        {
            var gzip = ParseGZipOptions(context);
            context.Content(STATUS_OK, text, CONTENT_TEXT_HTML, Encoding.UTF8, gzip);
        }

        public static void ReturnContent(this HttpListenerContext context, string text, string contentType, Encoding encoding)
        {
            var gzip = ParseGZipOptions(context);
            context.Content(STATUS_OK, text, contentType, encoding, gzip);
        }

        /// <summary>
        /// 204 - NoContent
        /// </summary>
        /// <param name="context"></param>
        public static void NoContent(this HttpListenerContext context)
        {
            Status(context, 204);
        }

        /// <summary>
        /// 403 - Forbidden
        /// </summary>
        /// <param name="context"></param>
        public static void Forbidden(this HttpListenerContext context)
        {
            Status(context, 403);
        }

        /// <summary>
        /// 404 - NotFound
        /// </summary>
        /// <param name="context"></param>
        public static void NotFound(this HttpListenerContext context)
        {
            Status(context, 404);
        }

        /// <summary>
        /// 503 - Service Unavailable
        /// </summary>
        /// <param name="context"></param>
        public static void ServiceNotImplemented(this HttpListenerContext context)
        {
            Status(context, 501);
        }

        /// <summary>
        /// 503 - Service Unavailable
        /// </summary>
        /// <param name="context"></param>
        public static void ServiceUnavailable(this HttpListenerContext context)
        {
            Status(context, 503);
        }

        public static void Status(this HttpListenerContext context, int code)
        {
            context.Content(code, "", CONTENT_TEXT_HTML, Encoding.UTF8);
        }

        private static bool ParseGZipOptions(HttpListenerContext context)
        {
            if (context?.Request != null)
            {
                var rencoding = context.Request.Headers["Accept-Encoding"];
                return !string.IsNullOrEmpty(rencoding) && rencoding.Contains("gzip");
            }

            return false;
        }

        public static void Content(
            this HttpListenerContext context,
            int code,
            string content,
            string contentType,
            Encoding encoding,
            bool gzipAllowed = false
        )
        {
            try
            {
                var response = context.Response;

                // 如果头部里有包含"GZIP”,"DEFLATE",表示你浏览器支持GZIP,DEFLATE压缩
                var bytes = ConvertBytes(content);
                var length = bytes.Length;
                var needZip = length >= 1024;
                var gzip = gzipAllowed && needZip;

                //var rencoding = request.Headers["Accept-Encoding"];
                //var gzip = (!string.IsNullOrEmpty(rencoding) && rencoding.Contains("gzip")) && needZip;
                ////var deflate = (!string.IsNullOrEmpty(rencoding) && rencoding.Contains("deflate")) && needZip;
                if (gzip)
                {
                    // 向输出流头部添加压缩信息
                    context.Response.AppendHeader("Content-Encoding", "gzip");

                    // 压缩字节
                    bytes = CompressBytes(bytes);
                    length = bytes.Length;
                }

                response.ContentLength64 = length;
                response.ContentType = contentType; // "text/html; charset=utf-8";
                response.ContentEncoding = encoding;

                response.StatusCode = code;
                response.KeepAlive = false;
                response.SendChunked = (length > CHUNKED_SIZE);

                var remain = length;
                var sent = 0;

                using (var output = response.OutputStream)
                {
                    if (length > 0)
                    {
                        while (remain > CHUNKED_SIZE)
                        {
                            output.Write(bytes, sent, CHUNKED_SIZE);
                            output.Flush();
                            remain -= CHUNKED_SIZE;
                            sent += CHUNKED_SIZE;
                        }

                        output.Write(bytes, sent, remain);
                        output.Flush();
                    }
                    output.Close();
                }

                response.Close();
            }
            catch (Exception ex)
            {
                Logger.Error("HttpListenerContext", ex);

                if (context != null)
                {
                    try
                    {
                        context.Response.Abort();
                        context.Response.Close();
                    }
                    // ReSharper disable EmptyGeneralCatchClause
                    catch { }
                    // ReSharper restore EmptyGeneralCatchClause
                }
            }
        }

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static byte[] CompressBytes(byte[] data)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var gz = new GZipStream(memoryStream, CompressionMode.Compress))
                    {
                        gz.Write(data, 0, data.Length);
                        gz.Close();
                    }

                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("HttpListenerContext", ex);
            }

            return data;
        }

        /// <summary>
        /// Convert Bytes
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static byte[] ConvertBytes(string result)
        {
            return Encoding.UTF8.GetBytes(result);
        }

        /// <summary>
        /// BUFFER SIZE
        /// </summary>
        private const int CHUNKED_SIZE = 819600;
    }
}
