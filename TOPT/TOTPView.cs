using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace TOPT
{
    public partial class TOTPView : UserControl
    {
        string strSecret = "";

        public TOTPView(string secret, string description = "")
        {
            InitializeComponent();
            strSecret = secret ?? throw new ArgumentNullException(nameof(secret));
            labelDescription.Text = description;
            btnDelete.Click += (s, e) =>
            {
                if (MessageBox.Show($"確定要刪除這個 {description} 嗎？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    RecordProvider.DeleteRecord(strSecret);
                    if (this.Parent.Parent is TOTPForm view)
                    {
                        view.BeginInvoke(() =>
                        {
                            view.InitializeContent();
                        });
                    }
                }
            };
        }

        private void TOTPView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Clipboard.SetText(labelTotp.Text);
            }
        }

        public void Ontick(long timestampSeconds)
        {
            long timeStep = timestampSeconds / 30;
            string totp = Crypto.GenerateTotp(strSecret, timeStep);
            var stepSeconds = timestampSeconds % 30; // 0 ~ 29
            labelTotp.Text = totp;
        }
    }
}
