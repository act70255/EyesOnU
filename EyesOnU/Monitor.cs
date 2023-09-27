using EyesOnU.Service.Compoment;
using EyesOnU.Controls;
using EyesOnU.Service.Extension;
using EyesOnU.Service;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Text;

namespace EyesOnU
{
    public partial class Monitor : FormBorderlessAndAlwaysTop
    {
        private static readonly Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        SystemMonitorService systemMonitorService = SystemMonitorService.Instance;
        List<CounterMonitor> CounterList = new List<CounterMonitor>();
        HttpClient httpClient = new HttpClient();
        public string RefreshRate
        {
            get
            {
                return config.AppSettings.Settings["RefreshRate"].Value;
            }
            set
            {
                var parsedValue = GetInt(value, 100);
                config.AppSettings.Settings["RefreshRate"].Value = parsedValue.ToString();
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                CounterList.ForEach(each => { each.RefreshRate = parsedValue; });
            }
        }
        public int GetInt(string key, int defaultValue = 0)
        {
            if (int.TryParse(key, out int result))
            {
                return result;
            }
            else
            {
                Debug.WriteLine($"[Int Parse failed] {key}");
                return defaultValue;
            }
        }
        public Monitor()
        {
            InitializeComponent();
            this.Shown += (s, e) =>
            {
                var refreshRate = GetInt(RefreshRate);
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
                    {
                        label.BeginInvoke((MethodInvoker)delegate () { label.Text = $"[{data.CounterType.GetDescription()}]\t - {e.Data}"; });

                        try
                        {
                            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                            var reaponse = httpClient.PostAsync("http://localhost:8888/Index/Data", content).GetAwaiter().GetResult();
                            Debug.WriteLine(reaponse);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                };
                this.pnlContent.Controls.Add(label);
                yPos += label.Height;
            }
            #endregion
            #region Operator
            #region TextBox RefreshRate
            TextBox txtRefreshRate = new TextBox
            {
                BackColor = Color.Black,
                ForeColor = Color.White,
                Width = 60,
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
            #endregion
            #region Button Exit
            CheckBox chkLock = new CheckBox
            {
                BackColor = Color.Black,
                ForeColor = Color.White,
                Width = 80,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "TopMost",
                Checked = this.TopMost,
                Location = new Point(txtRefreshRate.Right + 10, yPos),
            };
            chkLock.CheckedChanged += (s, e) =>
            {
                this.TopMost = chkLock.Checked;
            };
            this.pnlContent.Controls.Add(chkLock);
            #endregion
            #region Button Exit
            Button btnExit = new Button
            {
                BackColor = Color.Black,
                ForeColor = Color.White,
                Text = "Exit",
                Location = new Point(chkLock.Right + 10, yPos),
            };
            btnExit.Click += (s, e) =>
            {
                this.Close();
            };
            this.pnlContent.Controls.Add(btnExit);
            #endregion

            #endregion
            base.InitializeContent();
        }
    }
}