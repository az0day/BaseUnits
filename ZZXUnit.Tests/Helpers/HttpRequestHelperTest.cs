using System;
using Xunit;
using Xunit.Abstractions;

using BaseUnits.Core.Helpers;

namespace ZZXUnit.Tests.Helpers
{
    public class HttpRequestHelperTest : BaseTester
    {
        public HttpRequestHelperTest(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            

        }


        [Theory]
        [InlineData("http://novels.vps/", "在线小说阅读")]
        [InlineData("http://novels.vpss/", "No such host is known")]
        [InlineData("http://novels.vps/1233k3/kdio.html", "(404)")]
        public void Get_Html_Test(string url, string contains)
        {
            string html;
            var success = HttpRequestHelper.Get(url, out html);

            WriteLine("Url: {0}", url);
            WriteLine("Succss: {0}", success);
            WriteLine("HTML: {0}", html);
            WriteLine("");
            WriteLine("");

            var should = html.Contains(contains);
            Assert.True(should);
        }

        [Theory]
        [InlineData("http://novels.vps:8338/a/b/c", "/d/e/f.html")]
        [InlineData("http://novels.vps:8338/abc/", "../cde/a.html")]
        [InlineData("http://novels.vps:8338/abc/", "cde/../others/a.html")]
        [InlineData("http://novels.vps:8338/a/b/c.html", "/d/e/f.html")]
        [InlineData("http://novels.vps:8338/abc/index.html", "../cde/a.html")]
        [InlineData("http://novels.vps:8338/abc/abc.html", "cde/../others/a.html")]
        public void Create_Uri_Test(string baseUrl, string relative)
        {
            var uri = new Uri(baseUrl);
            var next = new Uri(uri, relative);

            WriteLine("baseUrl: {0}, relative: {1}", baseUrl, relative);
            WriteLine("AbsolutePath: {0}", next.AbsolutePath);
            WriteLine("AbsoluteUri: {0}", next.AbsoluteUri);
            WriteLine("Authority: {0}", next.Authority);
        }
    }
}
