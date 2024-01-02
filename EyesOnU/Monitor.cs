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
using Newtonsoft.Json.Linq;
using EyesOnU.Compoment;

namespace EyesOnU
{
    public partial class Monitor : FormBorderlessAndAlwaysTop
    {
        const float MAXFont = 50;
        const float MINFont = 5;
        private static readonly Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        private readonly SystemMonitorService systemMonitorService = SystemMonitorService.Instance;
        List<CounterMonitor> CounterList = new List<CounterMonitor>();

        private string RefreshRate
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

        private float FontSize
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

        private Color SettingBackColor
        {
            get
            {
                var color = config.AppSettings.Settings["BackColor"].Value;
                return ColorTranslator.FromHtml(color);
            }
        }

        private Color SettingForeColor
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

        private void ApplyFontSize(float fontSize)
        {
            var allElement = GetAllControls(pnlContent);
            foreach (var each in allElement)
            {
                if (each is Label label)
                    label.Font = new Font(label.Font.FontFamily, fontSize, label.Font.Style, label.Font.Unit, ((byte)(0)));
                else if (each is TextBox textBox)
                    textBox.Font = new Font(textBox.Font.FontFamily, fontSize, textBox.Font.Style, textBox.Font.Unit, ((byte)(0)));
                else if (each is Button button)
                    button.Font = new Font(button.Font.FontFamily, fontSize, button.Font.Style, button.Font.Unit, ((byte)(0)));
            }

            var ypos = 0;
            allElement.Where(f => f is Button).ToList().ForEach(each =>
            {
                each.Location = new Point(each.Location.X, ypos);
            });
            ypos = allElement.Where(f => f is Button).Max(m => m.Bottom);
            allElement.Where(f => f is TextBox || f is CheckBox).ToList().ForEach(each =>
            {
                each.Location = new Point(each.Location.X, ypos);
            });
            ypos = allElement.Where(f => f is TextBox || f is CheckBox).Max(m => m.Bottom);
            allElement.Where(f => f is Label).OrderBy(o => o.Top).ToList().ForEach(each =>
            {
                each.Location = new Point(each.Location.X, ypos);
                ypos += each.Height;
            });
        }

        protected override void InitializeContent()
        {
            this.pnlContent.AutoSize = true;
            this.pnlContent.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            int yPos = 0;
            CounterList = systemMonitorService.GetCounterMonitors();

            AddOperator(this.pnlContent);
            AddContent(this.pnlContent, CounterList);

            base.InitializeContent();
            return;

            void AddOperator(Control control)
            {
                #region Translate
                Button btnTranslate = new Button
                {
                    BackColor = SettingBackColor,
                    ForeColor = SettingForeColor,
                    Text = "Convert",
                    Location = new Point(0, yPos),
                };
                btnTranslate.Click += (s, e) =>
                {
                    var translate = new TranslateDialog(SettingBackColor, SettingForeColor);
                    translate.ShowDialog();
                };
                control.Controls.Add(btnTranslate);
                #endregion
                #region Button Exit
                Button btnExit = new Button
                {
                    BackColor = SettingBackColor,
                    ForeColor = SettingForeColor,
                    Text = "Exit",
                    Location = new Point(btnTranslate.Right + 150, yPos),
                };
                btnExit.Click += (s, e) =>
                {
                    this.Close();
                };
                control.Controls.Add(btnExit);
                #endregion
                yPos = control.Controls.OfType<Control>().Max(m => m.Bottom);
                #region TextBox RefreshRate
                TextBox txtRefreshRate = new TextBox
                {
                    BackColor = SettingBackColor,
                    ForeColor = SettingForeColor,
                    Width = 40,
                    Text = RefreshRate,
                    Location = new Point(0, yPos),
                    TextAlign = HorizontalAlignment.Center,
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
                txtRefreshRate.GotFocus += (s, e) =>
                {
                    txtRefreshRate.SelectAll();
                };
                control.Controls.Add(txtRefreshRate);
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
                control.Controls.Add(chkLock);
                #endregion
                #region TextBox RefreshRate
                TextBox txtFontSize = new TextBox
                {
                    BackColor = SettingBackColor,
                    ForeColor = SettingForeColor,
                    Width = 30,
                    Text = FontSize.ToString(),
                    TextAlign = HorizontalAlignment.Center,
                    Location = new Point(chkLock.Right + 10, yPos),
                };
                txtFontSize.TextChanged += (s, e) =>
                {
                    txtFontSize.Text = txtFontSize.Text.GetFloat(0).ToString();
                };
                txtFontSize.KeyPress += (s, e) =>
                {
                    if (e.KeyChar == (char)Keys.Enter && txtFontSize.Text.GetFloat() != FontSize)// && MessageBox.Show("Restart to Apply?", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        FontSize = txtFontSize.Text.GetFloat();
                        //Application.Restart();
                        ApplyFontSize(FontSize);
                        txtFontSize.SelectAll();
                    }
                    else if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    {
                        e.Handled = true;
                    }
                };
                txtFontSize.GotFocus += (s, e) =>
                {
                    txtFontSize.SelectAll();
                };
                control.Controls.Add(txtFontSize);
                #endregion
                yPos = control.Controls.OfType<Control>().Max(m => m.Bottom);
            }
            void AddContent(Control control, IEnumerable<CounterMonitor> counterList)
            {
                foreach (var each in counterList)
                {
                    Label label = new Label
                    {
                        AutoSize = true,
                        Text = @"Initializing...",
                        Font = new Font("Consolas", FontSize, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))),
                        BackColor = SettingBackColor,
                        ForeColor = SettingForeColor,
                        Location = new Point(0, yPos),
                    };

                    each.ValueUpdated += (s, e) =>
                    {
                        if (s is CounterMonitor data)
                        {
                            label.BeginInvoke((MethodInvoker)delegate () { label.Text = $@"[{data.CounterType.GetDescription()}]	 - {e.Data}"; });
                        }
                    };
                    control.Controls.Add(label);
                    yPos += label.Height;
                }
                yPos = control.Controls.OfType<Control>().Max(m => m.Bottom);
            }

        }
    }
}