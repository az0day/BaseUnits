using BaseUnits.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZZTests.DestopApp.Blls
{
    class BookBll : BaseBll
    {
        #region .Required Methods.
        public override void Initialize()
        {
            Thread.Sleep(1000);
        }

        public override void Open()
        {
            Thread.Sleep(1000);
        }

        public override void Close()
        {
            

        }

        protected override void Release()
        {
            

        }
        #endregion
    }
}
