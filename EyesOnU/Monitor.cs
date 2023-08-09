using EyesOnU.Service.Compoment;
using EyesOnU.Controls;
using EyesOnU.Service.Extension;
using EyesOnU.Service;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace EyesOnU
{
    public partial class Monitor : FormBorderlessAndAlwaysTop
    {
        private static readonly Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        SystemMonitorService systemMonitorService = SystemMonitorService.Instance;
        List<CounterMonitor> CounterList = new List<CounterMonitor>();
        public string RefreshRate
        {
            get
            {
                return config.AppSettings.Settings["RefreshRate"].Value;
            }
            set
            {
                config.AppSettings.Settings["RefreshRate"].Value = value;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                CounterList.ForEach(each => { each.RefreshRate = Convert.ToInt32(value); });
            }
        }
        public int GetInt(string key)
        {
            return Convert.ToInt32(RefreshRate);
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
            TextBox txtRefreshRate = new TextBox
            {
                BackColor = Color.Black,
                ForeColor = Color.White,
                Text = RefreshRate,
                Location = new Point(0, yPos),
                TextAlign = HorizontalAlignment.Right,
            };
            txtRefreshRate.TextChanged += (s, e) =>
            {
                txtRefreshRate.Text = Regex.Replace(txtRefreshRate.Text, @"[^0-9]", "");
            };
            txtRefreshRate.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            };
            txtRefreshRate.TextChanged += (s, e) =>
            {
                RefreshRate = txtRefreshRate.Text;
            };
            this.pnlContent.Controls.Add(txtRefreshRate);

            Button btnExit = new Button
            {
                BackColor = Color.Black,
                ForeColor = Color.White,
                Text = "Exit",
                Location = new Point(txtRefreshRate.Right + 50, yPos),
            };
            btnExit.Click += (s, e) =>
            {
                this.Close();
            };
            this.pnlContent.Controls.Add(btnExit);
            
            #endregion
            base.InitializeContent();
        }
    }
}