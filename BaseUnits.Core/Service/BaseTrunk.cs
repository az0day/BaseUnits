using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace BaseUnits.Core.Service
{
    public abstract class BaseTrunk<T> : BaseTrunk where T : new()
    {
        #region Properties, Fields and Events

        public static T Instance => InstanceLazy.Value;

        private static readonly Lazy<T> InstanceLazy = new Lazy<T>(() => new T(),
            LazyThreadSafetyMode.ExecutionAndPublication);

        #endregion
    }

    public abstract class BaseTrunk : BaseService
    {
        public IProgressState ProgressState { get; set; }

        private readonly IList<BaseBll> Blls;
        private readonly IList<BaseModule> Modules;

        /// <summary>
        /// 内部模块加载步数, 每个模块分2步
        /// </summary>
        public int Steps => (Modules.Count + Blls.Count) * 2 + 1;

        #region Constructor / Destructor
        protected BaseTrunk()
        {
            Blls = new List<BaseBll>();
            Modules = new List<BaseModule>();
        }
        #endregion

        #region Sealed Methods
        public sealed override void Initialize()
        {
            InitializePreHook();

            var form = ProgressState;
            foreach (var bll in Blls)
            {
                var preText = $"[{INITIALIZE_STEP}] {bll.Name}";

                bll.OnInvokeMessage += InvokeMessage;
                form?.Update($"{preText} {INITIALIZING}...");

                bll.Initialize();

                form?.Update($"{preText} {INITIALIZED}.");
                form?.Increase();
            }

            foreach (var module in Modules)
            {
                var preText = $"[{INITIALIZE_STEP}] {module.Name}";

                module.OnInvokeMessage += InvokeMessage;
                form?.Update($"{preText} {INITIALIZING}...");

                module.Initialize();

                form?.Update($"{preText} {INITIALIZED}.");
                form?.Increase();
            }

            InitializePostHook();
        }

        public sealed override void Open()
        {
            OpenPreHook();

            var form = ProgressState;

            foreach (var module in Modules)
            {
                var preText = $"[{OPEN_STEP}] {module.Name}";

                form?.Update($"{preText} {OPENING}...");
                module.Open();
                form?.Update($"{preText} {OPENED}.");
                form?.Increase();
            }

            foreach (var bll in Blls)
            {
                var preText = $"[{OPEN_STEP}] {bll.Name}";

                form?.Update($"{preText} {OPENING}...");
                bll.Open();
                form?.Update($"{preText} {OPENED}.");
                form?.Increase();
            }

            OpenPostHook();
        }

        public sealed override void Close()
        {
            ClosePreHook();

            var form = ProgressState;

            foreach (var bll in Blls.Reverse())
            {
                var preText = $"[{CLOSE_STEP}] {bll.Name}";

                form?.Update($"{preText} {CLOSING}...");
                bll.Close();
                form?.Update($"{preText} {CLOSED}.");
                form?.Increase();
            }

            foreach (var module in Modules.Reverse())
            {
                var preText = $"[{CLOSE_STEP}] {module.Name}";

                form?.Update($"{preText} {CLOSING}...");
                module.Close();
                form?.Update($"{preText} {CLOSED}.");
                form?.Increase();
            }

            ClosePostHook();
        }

        protected sealed override void Release()
        {
            ReleasePreHook();

            var form = ProgressState;

            foreach (var bll in Blls.Reverse())
            {
                var preText = $"[{RELEASE_STEP}] {bll.Name}";

                form?.Update($"{preText} {RELEASING}...");
                bll.OnInvokeMessage -= InvokeMessage;
                bll.Dispose();
                form?.Update($"{preText} {RELEASED}.");
                form?.Increase();
            }

            foreach (var module in Modules.Reverse())
            {
                var preText = $"[{RELEASE_STEP}] {module.Name}";

                form?.Update($"{preText} {RELEASING}...");
                module.OnInvokeMessage -= InvokeMessage;
                module.Dispose();
                form?.Update($"{preText} {RELEASED}.");
                form?.Increase();
            }

            ReleasePostHook();
        }
        #endregion

        #region Required Overriden Methods
        protected abstract void InitializePreHook();

        protected abstract void InitializePostHook();

        protected abstract void OpenPreHook();

        protected abstract void OpenPostHook();

        protected abstract void ClosePreHook();

        protected abstract void ClosePostHook();

        protected abstract void ReleasePreHook();

        protected abstract void ReleasePostHook();
        #endregion

        #region Public Methods
        protected void UseBll(BaseBll bll)
        {
            Blls.Add(bll);
        }

        protected void UseModule(BaseModule module)
        {
            Modules.Add(module);
        }

        public static string FormatToNumber(long num)
        {
            return $"{num:N0}";
        }

        public static string FormatToShortBytes(long num)
        {
            if (num >= 100000000M)
            {
                var newNum = num / 1000000M;
                return newNum.ToString("0.#") + "MB";
            }
            if (num >= 10000000M)
            {
                var newNum = num / 1000000M;
                return newNum.ToString("0.#0") + "MB";
            }
            if (num >= 1000000M)
            {
                var newNum = num / 1000000M;
                return newNum.ToString("0.##") + "MB";
            }
            if (num >= 100000M)
            {
                var newNum = num / 1000M;
                return newNum.ToString("0.#") + "KB";
            }
            if (num >= 10000M)
            {
                var newNum = num / 1000M;
                return newNum.ToString("0.0") + "KB";
            }
            if (num >= 1000M)
            {
                var newNum = num / 1000M;
                return newNum.ToString("0.#0") + "KB";
            }

            return num.ToString("#,0");
        }
        #endregion
    }
}
