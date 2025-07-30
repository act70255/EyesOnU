using Microsoft.VisualBasic.Devices;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DisplayHelper
{
    public partial class DisplayForm : Form, IMessageFilter
    {
        private static readonly Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        Dictionary<string, string> DisplayDatas = new Dictionary<string, string> { };

        #region Dragable
        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;
        internal HashSet<Control> controlsToMove = new HashSet<Control>();

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

        protected void SetDragable()
        {
            //Dragable
            Application.AddMessageFilter(this);
            #region Register dragable
            foreach (var ctrl in GetAllControls(this))
            {
                //設定可drag元件類別
                if (ctrl is Label || ctrl is Panel)
                {
                    controlsToMove.Add(ctrl);
                }
            }
            #endregion
        }

        public static IEnumerable<Control> GetAllControls(Control control)
        {
            var controls = control.Controls.Cast<Control>();
            return controls.SelectMany(ctrl => GetAllControls(ctrl))
                                          .Concat(controls);
        }
        #endregion
        #region WindowsLoaction
        private void SetWindowsLocation()
        {
            try
            {
                // 螢幕尺寸
                var screenSize = Screen.PrimaryScreen.Bounds;
                int margin = 10;
                int x = screenSize.Width - this.Width - margin;
                // 設定視窗垂直居中
                int y = (screenSize.Height - this.Height) / 2;
                this.Location = new Point(x, y);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        #endregion
        #region Config
        private float FontSize
        {
            get
            {
                if(float.TryParse(config.AppSettings.Settings["FontSize"].Value, out float fontSize))
                    return fontSize;
                else return 0f;
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
        #endregion

        public DisplayForm(Dictionary<string,string> contents)
        {
            InitializeComponent();

            DisplayDatas = contents;

            this.TopMost = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            chkLock.CheckedChanged += (sender, e) =>
            {
                this.TopMost = chkLock.Checked;
            };
            btnExit.Click += (sender, e) =>
            {
                this.Close();
            };
            this.Load += (s, e) =>
            {
                this.BackColor = SettingBackColor;
                this.ForeColor = SettingForeColor;
                pnlOperator.BackColor = SettingBackColor;
                pnlOperator.ForeColor = SettingForeColor;
                pnlContent.BackColor = SettingBackColor;
                pnlContent.ForeColor = SettingForeColor;
                lblMessage.BackColor = SettingBackColor;
                lblMessage.ForeColor = SettingForeColor;
                btnExit.BackColor = SettingBackColor;
                btnExit.ForeColor = SettingForeColor;
                InitializeContent();
                InitializeForm();
            };
            this.Shown += (s, e) =>
            {
                if (chkLock != null)
                    chkLock.Checked = true;
            };
        }

        private void InitializeContent()
        {
            int yPos = 0;
            //AddOperator(this.pnlContent);
            AddContent(DisplayDatas);

            return;
            //    yPos = control.Controls.OfType<Control>().Max(m => m.Bottom);

            void AddContent(Dictionary<string, string> counterList)
            {
                var control = this.pnlContent;
                yPos = 0;
                foreach (var each in counterList)
                {
                    Label label = new Label
                    {
                        AutoSize = true,
                        Text = $"[{each.Key}] {each.Value}",
                        Font = new Font("Consolas", FontSize, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))),
                        BackColor = SettingBackColor,
                        ForeColor = SettingForeColor,
                        Top = yPos,
                    };
                    label.MouseUp += (s, e) =>
                    {
                        lblMessage.Text = $"已複製 - {each.Value}";
                        Debug.WriteLine($"Label Clicked: {each.Key} - {each.Value}");
                        Clipboard.SetText(each.Value);
                    };
                    control.Controls.Add(label);
                    yPos += label.Height;
                }
                control.Controls.Add(new Label
                {
                    Text = "",
                    Height = 15, // Add a spacer
                    BackColor = SettingBackColor,
                    ForeColor = SettingForeColor,
                    Top = yPos,
                });
            }
        }
        private void InitializeForm()
        {
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Location = new Point(0, 0);
            SetDragable();
            SetWindowsLocation();
        }
    }
}
