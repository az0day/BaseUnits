using static BaseUnits.Core.Helpers.LogsHelper;

namespace BaseUnits.Core.Service
{
    public interface IMessageDisplay
    {
        bool Paused { get; set; }
        void Clear();
        void Push(string title, string content, LogLevelEnum level = LogLevelEnum.Info);
        void Push(string content, LogLevelEnum level = LogLevelEnum.Info);
    }
}
