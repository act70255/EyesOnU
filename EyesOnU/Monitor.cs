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
        const float MAXFont = 60;
        const float MINFont = 5;
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
                var parsedValue = value.GetInt(100);
                config.AppSettings.Settings["RefreshRate"].Value = parsedValue.ToString();
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                CounterList.ForEach(each => { each.RefreshRate = parsedValue; });
            }
        }

        public float FontSize
        {
            get
            {
                return config.AppSettings.Settings["FontSize"].Value.GetFloat(9);
            }
            set
            {
                value = value < MINFont ? MINFont : value;
                value = value > MAXFont ? MAXFont : value;

                config.AppSettings.Settings["FontSize"].Value = value.ToString();
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                pnlContent.Controls.OfType<Label>().ToList().ForEach(each => { each.Font = new Font(each.Font.FontFamily, value, each.Font.Style, each.Font.Unit, ((byte)(0))); });

                var allElement = GetAllControls(pnlContent);
                var bottomContent = allElement.Where(f => f is Label).Max(m => m.Bottom);
                allElement.Where(f => f is not Label).ToList().ForEach(each =>
                {
                    each.Top = bottomContent;
                });
            }
        }

        public Color SettingBackColor
        {
            get
            {
                var color = config.AppSettings.Settings["BackColor"].Value;
                return ColorTranslator.FromHtml(color);
            }
        }
        public Color SettingForeColor
        {
            get
            {
                var color = config.AppSettings.Settings["ForeColor"].Value;
                return ColorTranslator.FromHtml(color);
            }
        }

        public Monitor()
        {
            InitializeComponent();
            BackColor = SettingBackColor;
            this.Shown += (s, e) =>
            {
                var refreshRate = RefreshRate.GetInt();
                CounterList.ForEach(each => { Task.Factory.StartNew(() => { each.StartNext(refreshRate); }); });
                pnlContent.Controls.OfType<CheckBox>().FirstOrDefault()?.Focus();
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
                    Font = new Font("Consolas", FontSize, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))),
                    BackColor = SettingBackColor,
                    ForeColor = SettingForeColor,
                    Location = new Point(0, yPos),
                };
                //label.Font = new Font(label.Font.FontFamily, 15, label.Font.Style, label.Font.Unit, ((byte)(0)));
                each.ValueUpdated += (s, e) =>
                {
                    if (s is CounterMonitor data)
                    {
                        label.BeginInvoke((MethodInvoker)delegate () { label.Text = $"[{data.CounterType.GetDescription()}]\t - {e.Data}"; });

                        //try
                        //{
                        //    var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                        //    var reaponse = httpClient.PostAsync("http://localhost:8888/Index/Data", content).GetAwaiter().GetResult();
                        //    Debug.WriteLine(reaponse);
                        //}
                        //catch (Exception ex)
                        //{
                        //    Debug.WriteLine(ex);
                        //}
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
                BackColor = SettingBackColor,
                ForeColor = SettingForeColor,
                Width = 40,
                Text = RefreshRate,
                Location = new Point(0, yPos),
                TextAlign = HorizontalAlignment.Right,
            };
            txtRefreshRate.TextChanged += (s, e) =>
            {
                txtRefreshRate.Text = Regex.Replace(txtRefreshRate.Text, @"[^0-9]", "");
                RefreshRate = txtRefreshRate.Text;
            };
            txtRefreshRate.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            };
            this.pnlContent.Controls.Add(txtRefreshRate);
            #endregion
            #region CheckBox TopMost
            CheckBox chkLock = new CheckBox
            {
                BackColor = SettingBackColor,
                ForeColor = SettingForeColor,
                Width = 60,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Top",
                Checked = this.TopMost,
                Location = new Point(txtRefreshRate.Right + 10, yPos),
            };
            chkLock.CheckedChanged += (s, e) =>
            {
                this.TopMost = chkLock.Checked;
            };
            this.pnlContent.Controls.Add(chkLock);
            #endregion
            #region TextBox RefreshRate
            TextBox txtFontSize = new TextBox
            {
                BackColor = SettingBackColor,
                ForeColor = SettingForeColor,
                Width = 30,
                Text = FontSize.ToString(),
                TextAlign = HorizontalAlignment.Right,
                Location = new Point(chkLock.Right + 10, yPos),
            };
            txtFontSize.TextChanged += (s, e) =>
            {
                txtFontSize.Text = txtFontSize.Text.GetFloat(0).ToString();
            };
            txtFontSize.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter && txtFontSize.Text.GetFloat() != FontSize && MessageBox.Show("Restart to Apply?", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    FontSize = txtFontSize.Text.GetFloat();
                    Application.Restart();
                }
                else if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            };
            this.pnlContent.Controls.Add(txtFontSize);
            #endregion
            #region Button Exit
            Button btnExit = new Button
            {
                BackColor = SettingBackColor,
                ForeColor = SettingForeColor,
                Text = "Exit",
                Location = new Point(txtFontSize.Right + 10, yPos),
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