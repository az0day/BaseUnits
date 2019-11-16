using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BaseUnits.Core.Helpers;
using BaseUnits.Core.Service;

namespace BaseUnits.FormExtensions
{
    public class MessageDisplay : IMessageDisplay, IDisposable
    {
        private readonly TextBox _txtContent;
        private readonly ConcurrentQueue<string> _contentList = new ConcurrentQueue<string>();
        private readonly AutoResetEvent _updateEvent = new AutoResetEvent(false);

        /// <summary>
        /// 是否主窗体正在关闭
        /// </summary>
        private bool _isClosing;

        private void InitTextBox(TextBox textBox)
        {
            textBox.ReadOnly = true;
            textBox.Multiline = true;
            textBox.WordWrap = true;
            textBox.ScrollBars = ScrollBars.Vertical;

            var findForm = textBox.FindForm();
            if (findForm != null)
            {
                DefaultTitle = findForm.GetType().Name;
                findForm.FormClosing += FormClosing;
            }

            var menu = new ContextMenu();
            var menus = new MenuItem[]
            {
                new MenuItem("Pause(&P)", Pause),
                new MenuItem("Display(&D)", Display),
                new MenuItem("-"),
                new MenuItem("Clear(&E)", Clear),
                new MenuItem("-"),
                new MenuItem("Copy Content(&C)", Copy),
                new MenuItem("Export(&X)...", SaveAs),
                new MenuItem("-"),
                new MenuItem("About(&A)", GetAbout)
            };

            menu.MenuItems.AddRange(menus);
            textBox.ContextMenu = menu;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="control"></param>
        /// <param name="maxLines"></param>
        public MessageDisplay(TextBox control, int maxLines = 1000)
        {
            MaxLines = maxLines;

            _txtContent = control;
            InitTextBox(_txtContent);

            Task.Factory.StartNew(() =>
            {
                while (!_isClosing)
                {
                    if (_updateEvent.WaitOne())
                    {
                        Thread.Sleep(50);

                        if (Paused)
                        {
                            continue;
                        }

                        var content = GetAllContent();
                        Display(content);
                    }
                }
            });
        }

        #region .Public Properties.
        /// <summary>
        /// 标题
        /// </summary>
        public string DefaultTitle = string.Empty;

        /// <summary>
        /// 最大显示行数, 默认100
        /// </summary>
        public int MaxLines = 100;
        #endregion

        /// <summary>
        /// 清除内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clear(object sender, EventArgs e)
        {
            while (_contentList.Count > 0)
            {
                _contentList.TryDequeue(out var _);
            }

            Display("");
        }

        private string GetAllContent()
        {
            while (MaxLines > 0 && _contentList.Count > MaxLines)
            {
                _contentList.TryDequeue(out var _);
            }

            var content = string.Join(Environment.NewLine, _contentList);
            return content;
        }

        /// <summary>
        /// 复制内容到剪贴板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Copy(object sender, EventArgs e)
        {
            var text = _txtContent?.Text ?? "";
            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    Clipboard.SetText(text);
                }
                catch (Exception ex)
                {
                    Push("复制出错：" + ex.Message, LogsHelper.LogLevelEnum.Error);
                }
            }
        }

        private void SaveAs(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Title = "SaveFileDialog Export2File",
                Filter = "Text File (.log) | *.log",
                FileName = Process.GetCurrentProcess().ProcessName + ".log"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var filename = sfd.FileName;
                if (!string.IsNullOrWhiteSpace(filename))
                {
                    using (var sw = new StreamWriter(filename))
                    {
                        sw.WriteLine(GetAllContent());
                        sw.Close();
                    }
                }
            }
        }

        private static void GetAbout(object sender, EventArgs e)
        {
            MessageBox.Show("Message Display 1.0");
        }

        /// <summary>
        /// Pause
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pause(object sender, EventArgs e)
        {
            Paused = true;
        }

        /// <summary>
        /// Display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Display(object sender, EventArgs e)
        {
            Paused = false;
        }

        /// <summary>
        /// 显示日志
        /// </summary>
        /// <param name="message"></param>
        private void Display(string message)
        {
            _txtContent?.BeginInvoke((Action)delegate
            {
                if (!_isClosing)
                {
                    _txtContent.Text = message;
                    _txtContent.SelectionStart = message.Length;

                    //_txtContent.ScrollToBottom();
                    // https://stackoverflow.com/questions/1069497/how-to-scroll-down-in-a-textbox-by-code-in-c-sharp
                    _txtContent.ScrollToCaret();
                }
            });
        }

        /// <summary>
        /// 处理所在窗体正在关闭的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormClosing(object sender, FormClosingEventArgs e)
        {
            _isClosing = !e.Cancel;
        }

        #region .Implements.
        /// <summary>
        /// 是否暂停显示
        /// </summary>
        public bool Paused { get; set; }

        /// <summary>
        /// 清除所有内容
        /// </summary>
        public void Clear()
        {
            Clear(null, null);
        }

        public void Push(string title, string content, LogsHelper.LogLevelEnum level = LogsHelper.LogLevelEnum.Info)
        {
            if (!Paused)
            {
                var text = string.Format("[{0}] [{1}] ({2}) {3}", DateTime.Now.ToString("dd-HH:mm:ss"), level, title, content);
                _contentList.Enqueue(text);
                _updateEvent.Set();
            }
        }

        public void Push(string content, LogsHelper.LogLevelEnum level = LogsHelper.LogLevelEnum.Info)
        {
            Push(DefaultTitle, content, level);
        }

        public void Dispose()
        {
            _isClosing = true;
        }
        #endregion
    }
}
