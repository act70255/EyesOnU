using EyesOnU.Compoment;
using EyesOnU.Controls;
using EyesOnU.Service.Compoment;
using EyesOnU.Service.Extension;
using EyesOnU.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace EyesOnU
{
    public partial class DisplayForm : FormBorderlessAndAlwaysTop
    {
        private static readonly Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        Dictionary<string, string> DisplayDatas = new Dictionary<string, string> { };

        private float FontSize
        {
            get
            {
                return config.AppSettings.Settings["FontSize"].Value.GetFloat(14);
            }
            set
            {
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

        public DisplayForm(Dictionary<string, string> args)
        {
            InitializeComponent();
            //this.FormBorderStyle = FormBorderStyle.FixedSingle;
            DisplayDatas = args;
            BackColor = SettingBackColor;
            this.TopMost = true;

            this.Load += (s, e) =>
            {
                // 螢幕尺寸
                var screenSize = Screen.PrimaryScreen.Bounds;
                int margin = 10;
                int x = screenSize.Width - this.Width - margin;
                // 設定視窗垂直居中
                int y = (screenSize.Height - this.Height) / 2;
                this.Location = new Point(x, y);
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
            Label lblMessage = new Label
            {
                Font = new Font("", 12),
                AutoSize = true,
                BackColor = SettingBackColor,
                ForeColor = SettingForeColor,
            };

            AddOperator(this.pnlContent);
            AddContent(this.pnlContent, DisplayDatas);

            base.InitializeContent();
            return;
            void AddOperator(Control control)
            {
                Panel pnlOperator = new Panel
                {
                    Margin = new Padding(5, 5, 5, 5),
                    Dock = DockStyle.Top,
                    BackColor = SettingBackColor,
                    ForeColor = SettingForeColor,
                    BorderStyle = BorderStyle.FixedSingle,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                };
                control.Controls.Add(pnlOperator);
                #region Button Exit
                Button btnExit = new Button
                {
                    BackColor = SettingBackColor,
                    ForeColor = SettingForeColor,
                    Dock = DockStyle.Right,
                    //AutoSize = true,
                    Text = "Exit",
                    Height = 15
                };
                btnExit.Click += (s, e) =>
                {
                    this.Close();
                };
                pnlOperator.Controls.Add(btnExit);
                #endregion
                //yPos = control.Controls.OfType<Control>().Max(m => m.Bottom);
                #region CheckBox TopMost
                CheckBox chkLock = new CheckBox
                {
                    BackColor = SettingBackColor,
                    ForeColor = SettingForeColor,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Text = "維持在上面",
                    Checked = this.TopMost,
                    AutoSize = true,
                    Location = new Point(0 + 10, yPos),
                };
                chkLock.CheckedChanged += (s, e) =>
                {
                    this.TopMost = chkLock.Checked;
                };
                pnlOperator.Controls.Add(chkLock);
                lblMessage.Top = pnlOperator.Controls.OfType<Control>().Max(m => m.Bottom) + 5;
                Debug.WriteLine(lblMessage.Top);
                lblMessage.Left = 5;
                pnlOperator.Controls.Add(lblMessage);
                #endregion
                yPos = control.Controls.OfType<Control>().Max(m => m.Bottom);
            }
            void AddContent(Control control, Dictionary<string, string> counterList)
            {
                pnlContent.BorderStyle = BorderStyle.FixedSingle;
                foreach (var each in counterList)
                {
                    Label label = new Label
                    {
                        AutoSize = true,
                        Text = $"[{each.Key}] {each.Value}",
                        Font = new Font("Consolas", FontSize, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))),
                        BackColor = SettingBackColor,
                        ForeColor = SettingForeColor,
                        Location = new Point(0, yPos),
                    };
                    label.MouseUp += (s, e) =>
                    {
                        lblMessage.Text = $"已複製 - {each.Value}";
                        Debug.WriteLine($"Label Clicked: {each.Key} - {each.Value}");
                        Clipboard.SetText(each.Value);
                    };
                    //label.Click += (s, e) =>
                    //{
                    //    Clipboard.SetText(each.Value);
                    //};

                    control.Controls.Add(label);
                    yPos += label.Height;
                }
                yPos = control.Controls.OfType<Control>().Max(m => m.Bottom);
            }

        }

    }
}
