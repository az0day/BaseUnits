using ReverseMarkdown;

namespace BaseUnits.Markdowns
{
    public static class MarkdownExtensions
    {
        /// <summary>
        /// https://github.com/mysticmind/reversemarkdown-net
        /// 
        /// pass_through - Include the unknown tag completely into the result
        /// drop - Drop the unknown tag and its content
        /// bypass - Ignore the unknown tag but try to convert its content
        /// raise - Raise an error to let you know
        /// </summary>
        private static readonly Converter converter = new Converter(new Config
        {
            UnknownTags = Config.UnknownTagsOption.PassThrough, // Include the unknown tag completely in the result (default as well)
            GithubFlavored = true, // generate GitHub flavoured markdown, supported for BR, PRE and table tags
            RemoveComments = true, // will ignore all comments
            SmartHrefHandling = true // remove markdown output for links where appropriate
        });

        public static string ToMarkdown(this string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return "";
            }

            return converter.Convert(html);
        }

        public static string ToHtml(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "";
            }

            return CommonMark.CommonMarkConverter.Convert(text);
        }
    }
}
