using EyesOnU.Compoment;
using EyesOnU.Controls;
using EyesOnU.Extension;
using EyesOnU.Service;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EyesOnU
{
    public partial class Monitor : FormBorderlessAndAlwaysTop
    {
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

            this.Shown += (s, e) =>
            {
                var refreshRate = GetInt("RefreshRate");
                CounterList.ForEach(each => { Task.Factory.StartNew(() => { each.StartNext(refreshRate); }); });
            };
        }

        //void InitializeForm()
        //{
        //    this.AutoSize = true;
        //    this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        //    this.pnlContent.AutoSize = true;
        //    this.pnlContent.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        //    this.Location = new Point(0, 0);
        //    SetAlwaysOnTop();
        //    SetDragable();
        //}
        protected override void InitializeContent()
        {
            this.pnlContent.AutoSize = true;
            this.pnlContent.AutoSizeMode = AutoSizeMode.GrowAndShrink;
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
            base.InitializeContent();
        }

        public static IEnumerable<Control> GetAllControls(Control control)
        {
            var controls = control.Controls.Cast<Control>();
            return controls.SelectMany(ctrl => GetAllControls(ctrl))
                                          .Concat(controls);
        }
    }
}