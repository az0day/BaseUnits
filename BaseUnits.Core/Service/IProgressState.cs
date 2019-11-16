namespace BaseUnits.Core.Service
{
    /// <summary>
    /// 窗体进度
    /// </summary>
    public interface IProgressState
    {
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="state"></param>
        void Update(string state);

        /// <summary>
        /// 递进
        /// </summary>
        /// <param name="step"></param>
        void Increase(int step = 1);

        /// <summary>
        /// 完成
        /// </summary>
        void Complete(string state);

        /// <summary>
        /// 隐藏和释放
        /// </summary>
        /// <param name="dispose"></param>
        void End(bool dispose = true);
    }
}
