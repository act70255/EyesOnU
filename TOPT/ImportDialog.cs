using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TOTP
{
    public class ImportCodeDialog : Form
    {
        TextBox txtCode = new TextBox { Multiline = true, Size = new Size(360, 120), Location = new Point(10, 35), ScrollBars = ScrollBars.Vertical };
        public string InputText => txtCode.Text; // txt 需改為類別欄位

        public ImportCodeDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var lbl = new Label { Text = "請貼上匯出的OTP內容：", AutoSize = true, Location = new Point(10, 10) };
            var btnOK = new Button { Text = "匯入", DialogResult = DialogResult.OK, Location = new Point(210, 170), AutoSize = true };
            var btnCancel = new Button { Text = "取消", DialogResult = DialogResult.Cancel, Location = new Point(300, 170), AutoSize = true };
            this.Controls.AddRange([lbl, txtCode, btnOK, btnCancel]);
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
            TopMost = true;
            Text = "匯入OTP代碼";
            Size = new Size(400, 250);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
        }
    }

    public class ManualInputDialog : Form
    {
        TextBox txtDesc = new TextBox { Size = new Size(250, 23), Location = new Point(70, 58) };
        TextBox txtSecret = new TextBox { Size = new Size(250, 23), Location = new Point(70, 18) };
        public string Secret => txtSecret.Text; // txtSecret 需改為類別欄位
        public string Description => txtDesc.Text; // txtDesc 需改為類別欄位

        public ManualInputDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var lblSecret = new Label { Text = "密鑰：", AutoSize = true, Location = new Point(10, 20) };
            var lblDesc = new Label { Text = "描述：", AutoSize = true, Location = new Point(10, 70) };
            var btnOK = new Button { Text = "儲存", DialogResult = DialogResult.OK, Location = new Point(150, 110), AutoSize = true };
            var btnCancel = new Button { Text = "取消", DialogResult = DialogResult.Cancel, Location = new Point(230, 110), AutoSize = true };
            this.Controls.AddRange([lblSecret, txtSecret, lblDesc, txtDesc, btnOK, btnCancel]);
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
            TopMost = true;
            Text = "手動輸入OTP";
            Size = new Size(350, 200);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
        }
    }
}
