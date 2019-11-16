using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using BaseUnits.Core.Service;

namespace BaseUnits.FormExtensions.Forms
{
    public partial class ProgressForm : Form, IProgressState
    {
        private string _currentState;
        private int _currentProgress;
        private int _maxSteps;

        public ProgressForm(string title, int max = 100, Form owner = null)
        {
            InitializeComponent();

            if (owner != null)
            {
                Owner = owner;
            }
            else
            {
                Owner = null;
                try
                {
                    var list = Application.OpenForms;
                    var mainFormHandle = Process.GetCurrentProcess().MainWindowHandle;

                    foreach (Form item in list)
                    {
                        if (item.Handle == mainFormHandle)
                        {
                            Owner = item;
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            if (Owner != null)
            {
                Icon = Owner.Icon;
                StartPosition = FormStartPosition.Manual;
                var p = new Point
                {
                    X = Owner.Location.X + Owner.Size.Width / 2 - Width / 2,
                    Y = Owner.Location.Y + Owner.Size.Height / 2 - Height / 2
                };
                Location = p;
            }
            else
            {
                StartPosition = FormStartPosition.CenterScreen;
            }

            Text = title;

            _currentProgress = 1;
            _maxSteps = Math.Max(1, max);

            BarProgress.Step = 1;
            BarProgress.Minimum = 0;
            BarProgress.Maximum = _maxSteps;
            BarProgress.Value = 1;

            Update();
            BringToFront();
            Show(Owner);
        }

        public static IProgressState Create(string title, int max, Form owner = null)
        {
            return new ProgressForm(title, max, owner);
        }

        private void RefreshState()
        {
            if (Disposing || IsDisposed)
            {
                return;
            }

            BeginInvoke((Action)delegate
            {
                BarProgress.Value = Math.Min(_currentProgress, _maxSteps);
                TxtState.Text = _currentState;
            });
        }

        public void Complete(string state)
        {
            _currentProgress = _maxSteps;
            Update(state);
        }

        public void End(bool dispose = true)
        {
            if (!Visible)
            {
                return;
            }

            if (Disposing || IsDisposed)
            {
                return;
            }

            Invoke((Action)delegate
            {
                // 跨线程更新
                Hide();

                if (dispose)
                {
                    Dispose();
                }
            });
        }

        public void Increase(int step = 1)
        {
            Interlocked.Add(ref _currentProgress, step);
            RefreshState();
        }

        public void Update(string state)
        {
            _currentState = state;
            RefreshState();
        }
    }
}
