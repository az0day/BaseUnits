using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BaseUnits.Core.Helpers;
using BaseUnits.Core.Service;
using BaseUnits.FormExtensions;
using BaseUnits.FormExtensions.Forms;

namespace ZZTests.DestopApp
{
    public partial class FrmMain : Form
    {
        private readonly MessageDisplay _shower;
        private readonly Trunk _trunk = Trunk.Instance;
        private volatile bool _exit;

        public FrmMain()
        {
            var sw = Stopwatch.StartNew();
            LogsHelper.Debug("Init", "[MainForm] Ctor init.");

            InitializeComponent();

            // 其他组件
            _shower = new MessageDisplay(TxtMessages, 100);

            LogsHelper.Debug("Init", $"[MainForm] Ctor end. e={sw.ElapsedMilliseconds}ms");
        }

        private void ShowMessage(object sender, MessageEventArgs e)
        {
            ShowMessage(e.Name, e.Message, e.LogLevel);
        }

        private void ShowMessage(string title, string content, LogsHelper.LogLevelEnum level = LogsHelper.LogLevelEnum.Info)
        {
            _shower.Push(title, content, level);
        }

        private void Start()
        {
            // Start loading
            var done = false;

            var progress = ProgressForm.Create("DestopApp is initializing...", _trunk.Steps, this);

            _trunk.ProgressState = progress;
            _trunk.OnInvokeMessage += ShowMessage;

            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    _trunk.ProgressState?.Update("Starting...");
                    _trunk.Initialize();
                    _trunk.Open();

                    done = true;
                    ShowMessage(this, new MessageEventArgs("Main", "Trunk started successfully."));
                }
                catch (Exception ex)
                {
                    LogsHelper.Error("Init", $"Init Exception:{ex.ToString()} StackTrace:{ex.StackTrace}.");
                    MessageBox.Show(ex.ToString(), @"ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }

                _trunk.ProgressState?.Increase();
            });

            while (!done)
            {
                Application.DoEvents();
            }

            progress.Complete("Initialization done.");
            progress.End();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            Text = Text + $" (Version: {Application.ProductVersion} / Started: {DateTime.Now:yyyy-MM-dd HH:mm})";

            // 自动启动
            Start();

            // 消息测试
            ThreadTests(5);
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            const string MESSAGE = "Do you confirm to stop this service?";
            var result = MessageBox.Show(MESSAGE, @"DestopApp Message", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            // 先隐藏
            Hide();

            _exit = true;
            var done = false;
            var progress = ProgressForm.Create("Prepare to shutdown, please do not interrupt it.", _trunk.Steps, this);
            _trunk.ProgressState = progress;

            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {

                    _trunk.ProgressState?.Update("Shutdown...");
                    _trunk.Close();
                    _trunk.OnInvokeMessage -= ShowMessage;
                    _trunk.Dispose();

                    _trunk.ProgressState?.Increase();
                    _trunk.ProgressState?.Complete("Shutdown Complete.");

                }
                catch (Exception ex)
                {
                    LogsHelper.Error("Shutdown", $"Shutdown Exception:{ex.Message} StackTrace:{ex.StackTrace}.");
                    MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, @"ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    progress.End();
                }

                done = true;
            });

            while (!done)
            {
                Application.DoEvents();
            }

            Environment.Exit(0);
        }

        private void BtnClearMessage_Click(object sender, EventArgs e)
        {
            _shower.Clear();
        }

        private void ChkPauseMessages_CheckedChanged(object sender, EventArgs e)
        {
            _shower.Paused = ((CheckBox)sender).Checked;
        }

        private void BtnTestMessages_Click(object sender, EventArgs e)
        {
            ShowMessage("Test", StaticHelper.RandomKey(20));
        }

        private void ThreadTests(int count)
        {
            for(var i = 0; i < count; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    while (!_exit)
                    {
                        ShowMessage("Thread-Test", StaticHelper.RandomKey(20));
                        Thread.Sleep(StaticHelper.Random(10, 200));
                    }
                });
            }
        }
    }
}
