using System;

namespace BaseUnits.NetExtension
{
    /// <summary>
    /// HttpContext 上下文
    /// </summary>
    public class ReceivedEventArgs : EventArgs
    {
        public HttpContext Context { get; }

        public ReceivedEventArgs(HttpContext context)
        {
            Context = context;
        }
    }

    /// <summary>
    /// NoticeEventArgs 上下文
    /// </summary>
    public class NoticeEventArgs : EventArgs
    {
        public string Content { get; }
        public string Title { get; }

        public NoticeEventArgs(string title, string content)
        {
            Title = title;
            Content = content;
        }
    }
}
