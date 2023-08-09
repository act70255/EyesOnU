using EyesOnU.Compoment;
using EyesOnU.Extension;
using EyesOnU.Service;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EyesOnU
{
    public partial class Monitor : Form, IMessageFilter
    {
        #region Always On Top
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        #endregion
        #region Dragable
        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;
        private HashSet<Control> controlsToMove = new HashSet<Control>();

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN &&
                 controlsToMove.Contains(Control.FromHandle(m.HWnd)))
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                return true;
            }
            return false;
        }
        #endregion
        private static readonly Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        SystemMonitorService systemMonitorService = SystemMonitorService.Instance;
        List<CounterMonitor> CounterList = new List<CounterMonitor>();
        public int GetInt(string key)
        {
            var value = config.AppSettings.Settings[key]?.Value;
            return Convert.ToInt32(value);
        }
        public Monitor()
        {
            InitializeComponent();
            InitializeContent();
            InitializeForm();

            this.Shown += (s, e) =>
            {
                var refreshRate = GetInt("RefreshRate");
                CounterList.ForEach(each => { Task.Factory.StartNew(() => { each.StartNext(refreshRate); }); });
            };
        }

        void InitializeForm()
        {
            //Dragable
            Application.AddMessageFilter(this);
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.pnlContent.AutoSize = true;
            this.pnlContent.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Location = new Point(0, 0);
            #region Register dragable
            foreach (var ctrl in GetAllControls(this))
            {
                if (ctrl is not Button)
                {
                    controlsToMove.Add(ctrl);
                }
            } 
            #endregion
            #region AlwaysOnTop
            this.Load += (s, e) =>
              {
                  SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
              }; 
            #endregion
        }

        void InitializeContent()
        {
            int yPos = 0;
            #region Counter
            CounterList = systemMonitorService.GetCounterMonitors();
            foreach (var each in CounterList)
            {
                Label label = new Label
                {
                    AutoSize = true,
                    Text = "Initializing...",
                    Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    Location = new Point(0, yPos),
                };
                each.ValueUpdated += (s, e) =>
                {
                    if (s is CounterMonitor data)
                        label.BeginInvoke((MethodInvoker)delegate () { label.Text = $"[{data.CounterType.GetDescription()}]\t - {e.Data}"; });
                };
                this.pnlContent.Controls.Add(label);
                //controlsToMove.Add(label);
                yPos += label.Height;
            } 
            #endregion

            #region Operator
            Button btnExit = new Button
            {
                BackColor = Color.Black,
                ForeColor = Color.White,
                Text = "Exit",
                Location = new Point(0, yPos),
            };
            btnExit.Click += (s, e) =>
            {
                this.Close();
            };
            this.pnlContent.Controls.Add(btnExit); 
            #endregion
        }

        public static IEnumerable<Control> GetAllControls(Control control)
        {
            var controls = control.Controls.Cast<Control>();
            return controls.SelectMany(ctrl => GetAllControls(ctrl))
                                          .Concat(controls);
        }
    }
}