namespace BaseUnits.Markdowns
{
    public interface IMarkdown
    {
        /// <summary>
        /// 转成 Markdown 内容
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        string ToMarkdown(string html);

        /// <summary>
        /// 输出 HTML
        /// </summary>
        /// <param name="markdown"></param>
        /// <returns></returns>
        string ToHtml(string markdown);
    }
}
