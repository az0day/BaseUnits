using System;
using System.IO;
using System.Net;
using System.Text;
using BaseUnits.Core.Entities.Http;

namespace BaseUnits.Core.Helpers
{
    public static class HttpRequestHelper
    {
        #region .Privates.
        private static void UpdateCookies(CookieContainer container, CookieCollection ccs)
        {
            if (container != null && ccs != null)
            {
                if (ccs.Count > 0)
                {
                    container.Add(ccs);
                }
            }
        }

        private static string CreateRefererUrl(Uri uri)
        {
            var ab = uri.AbsoluteUri;
            var dp = ab.IndexOf('.');
            if (dp > 0)
            {
                var slash = ab.IndexOf("/", dp, StringComparison.Ordinal);
                if (slash > 0)
                {
                    return ab.Substring(0, slash);
                }
            }

            return ab;
        }
        #endregion

        #region .Methods.
        public static ContextResponse Parse(ContextRequest req)
        {
            string error;
            HttpWebRequest request = null;

            var url = req.Url;
            var data = req.PostData;
            var charset = req.RequestEncoding;

            var resp = new ContextResponse(req.Url, req.Container);
            try
            {
                var post = req.Method.Equals("POST", StringComparison.InvariantCultureIgnoreCase);
                var headers = req.Headers;
                var body = Encoding.UTF8.GetBytes(req.PostData);

                request = (HttpWebRequest)WebRequest.Create(req.Url);
                ServicePointManager.DefaultConnectionLimit = 50;

                request.Proxy = null;
                request.AllowWriteStreamBuffering = true;
                request.ProtocolVersion = HttpVersion.Version11;
                request.Method = req.Method;
                request.KeepAlive = false;
                request.ContentLength = body.Length;
                request.CookieContainer = req.Container;
                request.Referer = CreateRefererUrl(req.Url);
                request.Timeout = 20000;
                request.Headers.Set("Accept-Language", "en-US");

                if (!string.IsNullOrEmpty(req.Accept))
                {
                    request.Accept = req.Accept;
                }
                //request.Headers.Set("X-FORWARDED-FOR", "1.2.3.4");

                //request.ContentType = "application/json; charset=UTF-8";
                //request.Connection = "Keep-Alive";
                if (post)
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                }

                if (headers != null)
                {
                    foreach (var b in headers)
                    {
                        request.Headers[b.Key] = b.Value;
                    }
                }

                //request.Headers[HttpHeaders.Api] = api;
                //request.Headers[HttpHeaders.DataType] = dataType;
                //request.Headers[HttpHeaders.AcceptEncoding] = "gzip"; //support gzip 
                //request.Headers[HttpHeaders.Digest] = hash;
                //request.Headers[HttpHeaders.ClientHash] = serialKey;
                request.UserAgent = req.UserAgent;
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.SendChunked = false;

                if (post)
                {
                    using (var os = request.GetRequestStream())
                    {
                        os.Write(body, 0, body.Length);
                    }
                }

                // 返回响应
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var code = response.StatusCode;
                    if (code != HttpStatusCode.OK)
                    {
                        error = "No Details";

                        if (code == HttpStatusCode.InternalServerError)
                        {
                            var s = response.GetResponseStream();
                            if (s != null)
                            {
                                var objReader = new StreamReader(s, Encoding.UTF8);
                                error = objReader.ReadToEnd();
                                objReader.Close();
                            }
                        }

                        var html = $"Failed to parse content. code: {code}, Error: {error}";
                        return resp.FoundError(html);
                    }

                    // Cookies
                    var ccs = response.Cookies;
                    if (req.UpdateCookies)
                    {
                        UpdateCookies(req.Container, ccs);
                    }

                    var dc = response.CharacterSet;
                    if (!string.IsNullOrEmpty(dc) && (!dc.Equals("ISO-8859-1", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        try
                        {
                            charset = Encoding.GetEncoding(dc);
                        }
                        // ReSharper disable EmptyGeneralCatchClause
                        catch
                        // ReSharper restore EmptyGeneralCatchClause
                        {
                        }
                    }

                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            var sr = new StreamReader(responseStream, charset);
                            var responseData = sr.ReadToEnd();
                            sr.Close();

                            return resp.Success(responseData);
                        }
                    }
                }

                error = string.Format(
                    "Post [target: {0}, data: {1}] Error: No Response",
                    url,
                    data
                );
            }
            catch (WebException ex)
            {
                error = string.Format(
                    "Post [target: {0}, api: {1}, Error: {2}",
                    url,
                    data,
                    ex.Message
                );
            }
            finally
            {
                request.Abort();
            }

            // 处理错误
            return resp.FoundError(error);
        }

        /// <summary>
        /// Simple Parse Remote HTML
        /// </summary>
        /// <param name="url"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        public static bool Get(string url, out string html)
        {
            // 默认 UTF-8
            return Get(url, "", out html);
        }

        public static bool Get(string url, string encoding, out string html)
        {
            try
            {
                var enc = Encoding.UTF8;
                if ((!string.IsNullOrEmpty(encoding)) &&
                    (!encoding.Equals("UTF-8", StringComparison.InvariantCultureIgnoreCase)))
                {
                    enc = Encoding.GetEncoding(encoding);
                }

                var request = new ContextRequest(url)
                {
                    RequestEncoding = enc
                };

                var response = Parse(request);
                html = response.Html;
                return !response.Error;
            }
            catch (Exception ex)
            {
                html = ex.ToString();
                return false;
            }
        }
        #endregion

        #region .protected.
        public static ContextResponseStream Download(ContextRequest request)
        {
            var ct = request.Container;
            var response = new ContextResponseStream(request.Url, request.Container);

            try
            {
                var req = (HttpWebRequest)WebRequest.Create(request.Url);

                req.KeepAlive = true;
                req.Method = req.Method;
                req.AllowAutoRedirect = true;
                req.CookieContainer = ct;
                req.UserAgent = request.UserAgent;
                req.Timeout = 20000;

                if (!string.IsNullOrEmpty(req.Accept))
                {
                    request.Accept = req.Accept;
                }
                else
                {
                    req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/web";
                }

                ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) => true;

                var res = (HttpWebResponse)req.GetResponse();
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    var html = String.Format("Failed to parse content. code: {0}", res.StatusCode);
                    return response.FoundError(html);
                }

                var ccs = res.Cookies;
                if (request.UpdateCookies)
                {
                    UpdateCookies(request.Container, ccs);
                }

                var ret = res.GetResponseStream();
                var location = req.RequestUri;
                if (!location.Equals(request.Url))
                {
                    response.RedirectUrl(location);
                }

                return response.Success(ret);
            }
            catch (Exception ex)
            {
                return response.FoundError(ex.ToString());
            }
        }
        #endregion
    }
}
