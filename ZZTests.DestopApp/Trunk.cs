using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseUnits.Core.Service;
using ZZTests.DestopApp.Blls;
using ZZTests.DestopApp.Cache;
using ZZTests.DestopApp.Modules;

namespace ZZTests.DestopApp
{
    class Trunk : BaseTrunk<Trunk>
    {
        public readonly BookBll BookBll;
        public readonly OrderBll OrderBll;
        public readonly PaymentBll PaymentBll;
        public readonly UserBll UserBll;

        public readonly HttpApiModule HttpApi;
        public readonly RemotingClientModule RemotingClient;
        public readonly RemotingServerModule RemotingServer;
        public readonly TcpClientModule TcpClient;
        public readonly TcpServerModule TcpServer;

        public Trunk()
        {
            UseBll(BookBll = new BookBll());
            UseBll(OrderBll = new OrderBll());
            UseBll(PaymentBll = new PaymentBll());
            UseBll(UserBll = new UserBll());

            UseModule(HttpApi = new HttpApiModule());
            UseModule(RemotingClient = new RemotingClientModule());
            UseModule(RemotingServer = new RemotingServerModule());
            UseModule(TcpClient = new TcpClientModule());
            UseModule(TcpServer = new TcpServerModule());
        }
                
        private void InstanceOnNewMessage(object sender, MessageEventArgs args)
        {
            InvokeMessage(this, args);
        }

        #region Required Overriden Methods
        protected override void InitializePreHook()
        {
            try
            {
                // 统计信息
                StatsCountersFactory.Instance.Initialize();
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw;
            }

            OnlineUsersCache.Instance.OnInvokeMessage += InstanceOnNewMessage;
        }

        protected override void InitializePostHook()
        {
            try
            {
                // _tmrUpdatePendingPaymentStatus = new Timer(UpdatePendingPaymentStatusAction, null, 5000, 3 * 60 * 1000);
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw;
            }
        }

        protected override void OpenPostHook()
        {

        }

        protected override void OpenPreHook()
        {

        }

        protected override void ClosePreHook()
        {

        }

        protected override void ClosePostHook()
        {

        }

        protected override void ReleasePreHook()
        {

        }

        protected override void ReleasePostHook()
        {

        }
        #endregion
    }
}
