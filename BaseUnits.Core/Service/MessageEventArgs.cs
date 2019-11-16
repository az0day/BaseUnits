using System;
using LogLevels = BaseUnits.Core.Helpers.LogsHelper.LogLevelEnum;

namespace BaseUnits.Core.Service
{
    [Serializable]
    public class MessageEventArgs : EventArgs
    {
        public string Name { get; private set; }
        public string Message { get; private set; }
        public LogLevels LogLevel { get; private set; }

        public MessageEventArgs(string name, string message, LogLevels logLevel = LogLevels.Info)
        {
            Name = name;
            Message = message;
            LogLevel = logLevel;
        }
    }
}
