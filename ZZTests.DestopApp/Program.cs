using BaseUnits.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZZTests.DestopApp
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException += ApplicationThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            // 如果找到运行的实例则定位
            var instance = GetInstanceByModuleName();
            if (instance != null)
            {
                HandleRunningInstance(instance);
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FrmMain());
            }
        }

        /// <summary>
        /// Errors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogsHelper.Error("UnhandledException", e.ExceptionObject.ToString());
        }

        /// <summary>
        /// Errors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e)
        {
            LogsHelper.Error("ThreadException", e.Exception.ToString());
        }


        private const int WS_SHOWNORMAL = 1;

        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// 获取当前运行的实例
        /// </summary>
        /// <returns></returns>
        private static Process GetInstanceByModuleName()
        {
            // 获取当前进程
            // 创建新的 Process 组件的数组，并将它们与本地计算机上共享指定的进程名称的所有进程资源关联。
            var currentProcess = Process.GetCurrentProcess();
            //var currentFileName = currentProcess.MainModule.FileName;
            var currentFileName = currentProcess.MainModule.ModuleName;
            var processes = Process.GetProcessesByName(currentProcess.ProcessName);

            // 遍历正在有相同名字运行的进程
            foreach (var process in processes)
            {
                if (process.MainModule.ModuleName.Equals(currentFileName, StringComparison.OrdinalIgnoreCase))
                {
                    // 排除当前的进程
                    // 返回已启动的进程实例
                    if (process.Id != currentProcess.Id)
                    {
                        return process;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 获取窗口句柄
        /// </summary>
        /// <param name="instance">Process进程实例</param>
        private static void HandleRunningInstance(Process instance)
        {
            // 确保窗口没有被最小化或最大化
            ShowWindowAsync(instance.MainWindowHandle, WS_SHOWNORMAL);

            // 设置真实例程为foreground window
            SetForegroundWindow(instance.MainWindowHandle);
        }
    }
}
