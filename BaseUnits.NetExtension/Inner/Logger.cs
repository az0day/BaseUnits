using System;
using LogLevelEnum = BaseUnits.Core.Helpers.LogsHelper.LogLevelEnum;
using BaseUnits.Core.Helpers;

namespace BaseUnits.NetExtension.Inner
{
    internal static class Logger
    {
        private static void Write(string title, string content, LogLevelEnum level)
        {
            LogsHelper.Write(title, content, level);
        }

        public static void Debug(string title, string content)
        {
            Write(title, content, LogLevelEnum.Debug);
        }

        public static void Info(string title, string content)
        {
            Write(title, content, LogLevelEnum.Info);
        }

        public static void Error(string title, string content)
        {
            Write(title, content, LogLevelEnum.Error);
        }

        public static void Error(string title, Exception ex)
        {
            Write(title, ex.ToString(), LogLevelEnum.Error);
        }

        public static void Fatal(string title, string content)
        {
            Write(title, content, LogLevelEnum.Fatal);
        }

        public static void Warn(string title, string content)
        {
            Write(title, content, LogLevelEnum.Warn);
        }

        public static void Trace(string title, string content)
        {
            Write(title, content, LogLevelEnum.Trace);
        }
    }
}
