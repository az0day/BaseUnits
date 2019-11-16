using System;

namespace BaseUnits.Core
{
    /// <summary>
    /// 返回结果
    /// </summary>
    [Serializable]
    public class ResultObject<T>
    {
        /// <summary>
        /// 状态
        /// </summary>
        public StatusCodeEnum Code { get; set; }

        /// <summary>
        /// Return Object
        /// </summary>
        public T Ret { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 失败
        /// </summary>
        public bool Failed
        {
            get
            {
                return Code != StatusCodeEnum.Success;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ResultObject()
        {
            Code = StatusCodeEnum.Failed;
            Message = Code.ToString();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public ResultObject<T> Update(StatusCodeEnum status)
        {
            Code = status;
            Message = status.ToString();
            return this;
        }

        /// <summary>
        /// Update Object
        /// </summary>
        /// <param name="ret"></param>
        /// <returns></returns>
        public ResultObject<T> Update(T ret)
        {
            Ret = ret;
            return this;
        }

        /// <summary>
        /// Update Object & Status
        /// </summary>
        /// <param name="status"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        private ResultObject<T> Update(StatusCodeEnum status, T ret)
        {
            Code = status;
            Message = status.ToString();
            Ret = ret;
            return this;
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <param name="ret"></param>
        /// <returns></returns>
        public ResultObject<T> Success(T ret)
        {
            return Update(StatusCodeEnum.Success, ret);
        }

        public ResultObject<T> Info(string message)
        {
            Message = message;
            return this;
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="status"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ResultObject<T> Create(StatusCodeEnum status, T data)
        {
            return new ResultObject<T>
            {
                Code = status,
                Ret = data,
                Message = status.ToString()
            };
        }
    }
}
