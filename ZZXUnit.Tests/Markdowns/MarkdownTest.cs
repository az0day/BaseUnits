using Xunit;
using Xunit.Abstractions;

using BaseUnits.Markdowns;

namespace ZZXUnit.Tests.Markdowns
{
    public class MarkdownTest : BaseTester
    {
        public MarkdownTest(ITestOutputHelper outputHelper) : base(outputHelper)
        {


        }


        [Theory]
        [InlineData(@"This a sample <strong>paragraph</strong> from <a href=""http://test.com"">my site</a>", "This a sample **paragraph** from [my site](http://test.com)")]
        public void Get_Markdown_Test(string html, string text)
        {
            var result = html.ToMarkdown();

            WriteLine("html: {0}", html);
            WriteLine("text: {0}", result);

            Assert.Equal(text, result);
        }

        [Theory]
        [InlineData("This a sample **paragraph** from [hello](http://test.com)", @"<p>This a sample <strong>paragraph</strong> from <a href=""http://test.com"">hello</a></p>")]
        public void To_Html_Test(string markdown, string html)
        {
            var result = markdown.ToHtml();

            WriteLine("markdown: {0}", markdown);
            WriteLine("html: {0}", result);

            var eq = html.Equals(result);
            WriteLine("Equals: {0}", eq);
        }
    }
}
