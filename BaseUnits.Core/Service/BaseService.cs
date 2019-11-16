using System;

namespace BaseUnits.Core.Service
{
    public abstract class BaseService : BaseServiceWithMessage, IDisposable
    {
        public const string INITIALIZING = "initializing";
        public const string INITIALIZED = "initialized";

        public const string OPENING = "opening";
        public const string OPENED = "opened";

        public const string CLOSING = "closing";
        public const string CLOSED = "closed";

        public const string RELEASING = "releasing";
        public const string RELEASED = "released";

        public const string INITIALIZE_STEP = "1/2";
        public const string CLOSE_STEP = "1/2";
        public const string OPEN_STEP = "2/2";
        public const string RELEASE_STEP = "2/2";

        #region Destructor
        ~BaseService()
        {
            Dispose(false);
        }
        #endregion

        #region Required Methods
        public abstract void Initialize();
        public abstract void Open();
        public abstract void Close();
        protected abstract void Release();
        #endregion


        #region IDispose Methods
        private bool _disposed;

        public virtual void Dispose()
        {
            if (Equals(null))
            {
                return;
            }

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                Release();
            }

            _disposed = true;
        }
        #endregion
    }
}
