using System;
using System.Collections.Generic;
using BaseUnits.FormExtensions.Diagnostics;
using BaseUnits.Core.Diagnostics;
using BaseUnits.Core.Helpers;

namespace ZZTests.DestopApp
{
    internal sealed class StatsCountersFactory
    {
        public const string CATEGORY = ".ZZTests.DestopApp Statistics v1.0";
        public const string CATEGORY_HELP = "BaseUnits DestopApp Statistics";

        public static readonly StatsCountersFactory Instance = new StatsCountersFactory();

        public const string REMOTING_SERVICE_CLIENT_CONNECTIONS = "RemotingServiceClient Connections";
        public const string REMOTING_SERVICE_CLIENT_RECEIVED_PER_SECOND = "RemotingServiceClient Messages Received/sec";
        public const string REMOTING_SERVICE_CLIENT_RECEIVED = "RemotingServiceClient Messages Received";
        public const string REMOTING_SERVICE_CLIENT_SENT_PER_SECOND = "RemotingServiceClient Messages Sent/sec";
        public const string REMOTING_SERVICE_CLIENT_SENT = "RemotingServiceClient Messages Sent";

        public const string REMOTING_CLIENT_CONNECTIONS = "RemotingClient Connections";
        public const string REMOTING_CLIENT_RECEIVED_PER_SECOND = "RemotingClient Messages Received/sec";
        public const string REMOTING_CLIENT_RECEIVED = "RemotingClient Messages Received";
        public const string REMOTING_CLIENT_SENT_PER_SECOND = "RemotingClient Messages Sent/sec";
        public const string REMOTING_CLIENT_SENT = "RemotingClient Messages Sent";

        public const string TCP_SERVER_CONNECTIONS = "TcpServerClient Connections";
        public const string TCP_SERVER_RECEIVED_PER_SECOND = "TcpServerClient Messages Received/sec";
        public const string TCP_SERVER_RECEIVED = "TcpServerClient Messages Received";
        public const string TCP_SERVER_SENT_PER_SECOND = "TcpServerClient Messages Sent/sec";
        public const string TCP_SERVER_SENT = "TcpServerClient Messages Sent";

        public const string TCP_SERVICE_CLIENT_CONNECTIONS = "TcpClient Connections";
        public const string TCP_SERVICE_CLIENT_RECEIVED_PER_SECOND = "TcpClient Messages Received/sec";
        public const string TCP_SERVICE_CLIENT_RECEIVED = "TcpClient Messages Received";
        public const string TCP_SERVICE_CLIENT_SENT_PER_SECOND = "TcpClient Messages Sent/sec";
        public const string TCP_SERVICE_CLIENT_SENT = "TcpClient Messages Sent";

        public const string API_WEB_SERVER_CONNECTIONS = "ApiWebServer Connections";
        public const string API_WEB_SERVER_RECEIVED_PER_SECOND = "ApiWebServer Messages Received/sec";
        public const string API_WEB_SERVER_RECEIVED = "ApiWebServer Messages Received";
        public const string API_WEB_SERVER_SENT_PER_SECOND = "ApiWebServer Messages Sent/sec";
        public const string API_WEB_SERVER_SENT = "ApiWebServer Messages Sent";

        private readonly Dictionary<string, StatCounterType> _counters
            = new Dictionary<string, StatCounterType>
            {
                { API_WEB_SERVER_CONNECTIONS, StatCounterType.NumberOfItems32 },
                { API_WEB_SERVER_RECEIVED_PER_SECOND, StatCounterType.RateOfCountsPerSecond32 },
                { API_WEB_SERVER_RECEIVED, StatCounterType.NumberOfItems32 },
                { API_WEB_SERVER_SENT_PER_SECOND, StatCounterType.RateOfCountsPerSecond32 },
                { API_WEB_SERVER_SENT, StatCounterType.NumberOfItems32 },

                { TCP_SERVER_CONNECTIONS, StatCounterType.NumberOfItems32 },
                { TCP_SERVER_RECEIVED_PER_SECOND, StatCounterType.RateOfCountsPerSecond32 },
                { TCP_SERVER_RECEIVED, StatCounterType.NumberOfItems32 },
                { TCP_SERVER_SENT_PER_SECOND, StatCounterType.RateOfCountsPerSecond32 },
                { TCP_SERVER_SENT, StatCounterType.NumberOfItems32 },

                { REMOTING_SERVICE_CLIENT_CONNECTIONS, StatCounterType.NumberOfItems32 },
                { REMOTING_SERVICE_CLIENT_RECEIVED_PER_SECOND, StatCounterType.RateOfCountsPerSecond32 },
                { REMOTING_SERVICE_CLIENT_RECEIVED, StatCounterType.NumberOfItems32 },
                { REMOTING_SERVICE_CLIENT_SENT_PER_SECOND, StatCounterType.RateOfCountsPerSecond32 },
                { REMOTING_SERVICE_CLIENT_SENT, StatCounterType.NumberOfItems32 },

                { TCP_SERVICE_CLIENT_CONNECTIONS, StatCounterType.NumberOfItems32 },
                { TCP_SERVICE_CLIENT_RECEIVED_PER_SECOND, StatCounterType.RateOfCountsPerSecond32 },
                { TCP_SERVICE_CLIENT_RECEIVED, StatCounterType.NumberOfItems32 },
                { TCP_SERVICE_CLIENT_SENT_PER_SECOND, StatCounterType.RateOfCountsPerSecond32 },
                { TCP_SERVICE_CLIENT_SENT, StatCounterType.NumberOfItems32 },

                { REMOTING_CLIENT_CONNECTIONS, StatCounterType.NumberOfItems32 },
                { REMOTING_CLIENT_RECEIVED_PER_SECOND, StatCounterType.RateOfCountsPerSecond32 },
                { REMOTING_CLIENT_RECEIVED, StatCounterType.NumberOfItems32 },
                { REMOTING_CLIENT_SENT_PER_SECOND, StatCounterType.RateOfCountsPerSecond32 },
                { REMOTING_CLIENT_SENT, StatCounterType.NumberOfItems32 },
             };

        public void Initialize()
        {
            CleanStatsCategories();
            StatsCounterWin.Initialize(CATEGORY, CATEGORY_HELP, _counters);
        }

        private void CleanStatsCategories()
        {
            var categories = new List<string>
            {
                // "Ops per second (Tester)",
                CATEGORY,
            };

            try
            {
                StatsCounterWin.DeleteCategories(categories);
                LogsHelper.Info("StatsCounterCategory", $"cleaned: {categories.ToJson()}");
            }
            catch (Exception ex)
            {
                // only administrator has access
                // Ignored
                LogsHelper.Fatal("StatsCounterCategory", $"failed to clean: {categories.ToJson()}{Environment.NewLine}{ex}");
            }
        }
    }
}
