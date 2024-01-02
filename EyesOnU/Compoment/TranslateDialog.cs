using EyesOnU.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EyesOnU.Compoment
{
    public class TranslateDialog : FormBorderlessAndAlwaysTop
    {
        private const int LocaleSystemDefault = 0x0800;
        private const int LcmapSimplifiedChinese = 0x02000000;
        private const int LcmapTraditionalChinese = 0x04000000;


        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LCMapString(int locale, int dwMapFlags, string lpSrcStr, int cchSrc, string lpDestStr, int cchDest);

        private Panel pnlContent = new Panel()
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        TextBox txtInput = new TextBox()
        {
            Multiline = true,
            Size = new Size(260, 90)
        };
        TextBox txtOutput = new TextBox()
        {
            ReadOnly = true,
            AutoSize = true,
            Multiline = true,
            MinimumSize = new Size(260, 30)
        };
        Button btnExit = new Button()
        {
            Text = "Exit",
        };

        public Color SettingBackColor { get; }
        public Color SettingForeColor { get; }
        public TranslateDialog(Color backColor, Color foreColor)
        {
            SettingBackColor = backColor;
            SettingForeColor = foreColor;
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            pnlContent.BackColor = SettingBackColor;
            int ypos = 0;
            foreach (Control ctrl in pnlContent.Controls)
            {
                ctrl.BackColor = SettingBackColor;
                ctrl.ForeColor = SettingForeColor;
                ctrl.GetType().GetProperty("Location").SetValue(ctrl, new System.Drawing.Point(0, ypos));
                ypos = ctrl.Bottom + 3;
            }

            btnExit.Click += (s, e) => { this.Close(); };
            txtInput.TextChanged += (s, e) => { txtOutput.Text = ToSimplified(txtInput.Text); };
            txtOutput.Click+=(s,e) => { Clipboard.SetText(txtOutput.Text); };
        }

        /// <summary>
        /// 繁體轉簡體
        /// </summary>
        private static string ToSimplified(string argSource)
        {
            var t = new string(' ', argSource.Length);
            LCMapString(LocaleSystemDefault, LcmapSimplifiedChinese, argSource, argSource.Length, t, argSource.Length);
            return t;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TranslateDialog
            // 
            this.Name = "TranslateDialog";
            this.Size = new System.Drawing.Size(260, 90);
            this.Controls.Add(pnlContent);
            pnlContent.Controls.Add(txtInput);
            pnlContent.Controls.Add(txtOutput);
            pnlContent.Controls.Add(btnExit);
            this.ResumeLayout(false);
        }
    }
}
