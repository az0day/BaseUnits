using System;
using System.Threading;
using BaseUnits.Core.Helpers;
using MessageLevels = BaseUnits.Core.Helpers.LogsHelper.MessageLevelEnum;
using LogLevels = BaseUnits.Core.Helpers.LogsHelper.LogLevelEnum;

namespace BaseUnits.Core.Service
{
    public abstract class BaseServiceWithMessage
    {
        public string Name { get; protected set; }

        private int _exceptionsCount;
        public virtual long ExceptionsCount => _exceptionsCount;

        public virtual MessageLevels MessageLevel { get; protected set; } = MessageLevels.None;
        public virtual bool MessageEnable
        {
            get => MessageLevel == MessageLevels.Both || MessageLevel == MessageLevels.MsgOnly;
        }

        /// <summary>
        /// 消息处理
        /// </summary>
        public event EventHandler<MessageEventArgs> OnInvokeMessage;

        #region Constructor
        protected BaseServiceWithMessage()
        {
            var type = GetType().ToString().Split('.');
            Name = type[type.Length - 1];
        }
        #endregion

        #region Protected Methods
        protected void InvokeMessage(object sender, MessageEventArgs eventArgs)
        {
            OnInvokeMessage?.Invoke(sender, eventArgs);
        }

        protected virtual void LogInfo(string message, MessageLevels debugType = MessageLevels.MsgOnly)
        {
            LogMessage(Name, message, debugType);
        }

        protected virtual void LogInfo(string name, string message, MessageLevels debugType = MessageLevels.MsgOnly)
        {
            LogMessage(name, message, debugType);
        }

        protected virtual void LogInfo(ICorrelationIdLog cid, string message, MessageLevels debugType = MessageLevels.MsgOnly)
        {
            LogMessage(cid, Name, message, debugType);
        }

        protected virtual void LogInfo(ICorrelationIdLog cid, string name, string message, MessageLevels debugType = MessageLevels.MsgOnly)
        {
            LogMessage(cid, name, message, debugType);
        }

        protected virtual void LogDebug(string message, MessageLevels debugType = MessageLevels.Both)
        {
            LogMessage(Name, message, debugType, LogLevels.Debug);
        }

        protected virtual void LogDebug(string name, string message, MessageLevels debugType = MessageLevels.Both)
        {
            LogMessage(name, message, debugType, LogLevels.Debug);
        }

        protected virtual void LogDebug(ICorrelationIdLog cid, string message, MessageLevels debugType = MessageLevels.Both)
        {
            LogMessage(cid, Name, message, debugType, LogLevels.Debug);
        }

        protected virtual void LogDebug(ICorrelationIdLog cid, string name, string message, MessageLevels debugType = MessageLevels.Both)
        {
            LogMessage(cid, name, message, debugType, LogLevels.Debug);
        }

        protected virtual void LogTrace(string message, MessageLevels debugType = MessageLevels.LogOnly)
        {
            LogMessage(Name, message, debugType, LogLevels.Trace);
        }

        protected virtual void LogTrace(string name, string message, MessageLevels debugType = MessageLevels.LogOnly)
        {
            LogMessage(name, message, debugType, LogLevels.Trace);
        }

        protected virtual void LogTrace(ICorrelationIdLog cid, string message, MessageLevels debugType = MessageLevels.LogOnly)
        {
            LogMessage(cid, Name, message, debugType, LogLevels.Trace);
        }

        protected virtual void LogTrace(ICorrelationIdLog cid, string name, string message, MessageLevels debugType = MessageLevels.LogOnly)
        {
            LogMessage(cid, name, message, debugType, LogLevels.Trace);
        }

        protected virtual void LogWarn(string message, MessageLevels debugType = MessageLevels.Both)
        {
            LogMessage(Name, message, debugType, LogLevels.Warn);
        }

        protected virtual void LogWarn(string name, string message, MessageLevels debugType = MessageLevels.Both)
        {
            LogMessage(name, message, debugType, LogLevels.Warn);
        }

        protected virtual void LogWarn(ICorrelationIdLog cid, string message, MessageLevels debugType = MessageLevels.Both)
        {
            LogMessage(cid, Name, message, debugType, LogLevels.Warn);
        }

        protected virtual void LogWarn(ICorrelationIdLog cid, string name, string message, MessageLevels debugType = MessageLevels.Both)
        {
            LogMessage(cid, name, message, debugType, LogLevels.Warn);
        }

        protected virtual void LogError(string message, MessageLevels debugType = MessageLevels.Both)
        {
            LogMessage(Name, message, debugType, LogLevels.Error);
        }

        protected virtual void LogError(string name, string message, MessageLevels debugType = MessageLevels.Both)
        {
            LogMessage(name, message, debugType, LogLevels.Error);
        }

        protected virtual void LogError(ICorrelationIdLog cid, string message, MessageLevels debugType = MessageLevels.Both)
        {
            LogMessage(cid, Name, message, debugType, LogLevels.Error);
        }

        protected virtual void LogError(ICorrelationIdLog cid, string name, string message, MessageLevels debugType = MessageLevels.Both)
        {
            LogMessage(cid, name, message, debugType, LogLevels.Error);
        }

        protected virtual void LogFatal(string message, MessageLevels debugType = MessageLevels.Both)
        {
            LogMessage(Name, message, debugType, LogLevels.Fatal);
        }

        protected virtual void LogFatal(string name, string message, MessageLevels debugType = MessageLevels.Both)
        {
            LogMessage(name, message, debugType, LogLevels.Fatal);
        }

        protected virtual void LogFatal(ICorrelationIdLog cid, string message, MessageLevels debugType = MessageLevels.Both)
        {
            LogMessage(cid, Name, message, debugType, LogLevels.Fatal);
        }

        protected virtual void LogFatal(ICorrelationIdLog cid, string name, string message, MessageLevels debugType = MessageLevels.Both)
        {
            LogMessage(cid, name, message, debugType, LogLevels.Fatal);
        }

        protected virtual void LogMessage(string name, string message,
            MessageLevels debugType = MessageLevels.MsgOnly, LogLevels logLevel = LogLevels.Info)
        {
            switch (debugType)
            {
                case MessageLevels.Both:
                    OnInvokeMessage?.Invoke(this, new MessageEventArgs(name, message, logLevel));
                    LogsHelper.Write(name, message, logLevel);
                    break;
                case MessageLevels.LogOnly:
                    LogsHelper.Write(name, message, logLevel);
                    break;
                case MessageLevels.MsgOnly:
                    OnInvokeMessage?.Invoke(this, new MessageEventArgs(name, message, logLevel));
                    break;
            }
        }

        protected virtual void LogMessage(ICorrelationIdLog cid, string name, string message,
            MessageLevels debugType = MessageLevels.MsgOnly, LogLevels logLevel = LogLevels.Info)
        {
            switch (debugType)
            {
                case MessageLevels.Both:
                    OnInvokeMessage?.Invoke(this, new MessageEventArgs(name, $"[{cid}] {message}", logLevel));
                    LogsHelper.Write(name, $"[{cid}] {message}", logLevel);
                    break;
                case MessageLevels.LogOnly:
                    LogsHelper.Write(name, $"[{cid}] {message}", logLevel);
                    break;
                case MessageLevels.MsgOnly:
                    OnInvokeMessage?.Invoke(this, new MessageEventArgs(name, $"[{cid}] {message}", logLevel));
                    break;
            }
        }

        protected virtual void LogException(Exception ex,
            MessageLevels debugType = MessageLevels.Both, LogLevels logLevel = LogLevels.Fatal)
        {
            LogMessage(Name, $"An exception happened. Please check below for details.{Environment.NewLine}{ex}", debugType, logLevel);
            Interlocked.Increment(ref _exceptionsCount);
        }

        protected virtual void LogException(Exception ex, string value,
            MessageLevels debugType = MessageLevels.Both, LogLevels logLevel = LogLevels.Fatal)
        {
            LogMessage(Name, $"An exception happened. Please check below for details.{Environment.NewLine}ex={ex}{Environment.NewLine}value={value}",
                debugType, logLevel);
            Interlocked.Increment(ref _exceptionsCount);
        }

        protected virtual void LogException(ICorrelationIdLog cid, Exception ex,
            MessageLevels debugType = MessageLevels.Both, LogLevels logLevel = LogLevels.Fatal)
        {
            LogMessage(cid, Name, $"An exception happened. Please check below for details.{Environment.NewLine}{ex}", debugType, logLevel);
            Interlocked.Increment(ref _exceptionsCount);
        }

        protected virtual void LogException(ICorrelationIdLog cid, Exception ex, string value,
            MessageLevels debugType = MessageLevels.Both, LogLevels logLevel = LogLevels.Fatal)
        {
            LogMessage(cid, Name, $"An exception happened. Please check below for details.{Environment.NewLine}ex={ex}{Environment.NewLine}value={value}",
                debugType, logLevel);
            Interlocked.Increment(ref _exceptionsCount);
        }
        #endregion
    }
}
