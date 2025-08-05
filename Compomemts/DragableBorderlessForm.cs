using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TOTP
{
    public partial class DragableBorderlessForm : Form, IMessageFilter
    {
        private static readonly Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
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
                return GetConfig<float>("FontSize", 12f);
            }
        }

        private Color SettingBackColor
        {
            get
            {
                return GetConfig<Color>("BackColor", Color.Black);
            }
        }

        private Color SettingForeColor
        {
            get
            {
                return GetConfig<Color>("ForeColor", Color.Wheat);
            }
        }

        private T GetConfig<T>(string key, T defaultValue = default)
        {
            if (config.AppSettings.Settings[key] == null)
                return defaultValue;

            var value = config.AppSettings.Settings[key]?.Value;
            if (value == null)
                return defaultValue;
            if (typeof(T) == typeof(Color))
                return (T)(object)ColorTranslator.FromHtml(value);
            else if (typeof(T) == typeof(float))
                return (T)(object)float.Parse(value);
            else if (typeof(T) == typeof(int))
                return (T)(object)int.Parse(value);
            else if (typeof(T) == typeof(bool))
                return (T)(object)bool.Parse(value);
            else
                return (T)(object)value;
        }
        #endregion
        public DragableBorderlessForm()
        {
            InitializeComponent();

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

        public virtual void InitializeContent()
        {
            int yPos = 0;
            //AddOperator(this.pnlContent);
            

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

        public void AddContent(Control control, int spacing = 3, bool setTheme = false)
        {
            var contentHeight = pnlContent.Controls.OfType<Control>().Any() ? pnlContent.Controls.OfType<Control>().Max(c => c.Bottom): 0;
            control.Location = new Point(0, contentHeight);
            if (setTheme)
            {
                control.BackColor = SettingBackColor;
                control.ForeColor = SettingForeColor;
            }
            pnlContent.Controls.Add(control);
        }

        public void ClearContent()
        {
            pnlContent.Controls.Clear();
        }

        protected virtual void InitializeForm()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            
            SetDragable();
            SetWindowsLocation();
        }
    }
}
