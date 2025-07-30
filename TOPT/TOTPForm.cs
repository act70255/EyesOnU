using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Timer = System.Windows.Forms.Timer;

namespace TOPT
{
    public partial class TOTPForm : DragableBorderlessForm
    {
        Label lblTimer;
        public TOTPForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            //this.BackColor = Color.Black;
            //this.TransparencyKey = Color.Black;
            lblTimer = new Label
            {
                Text = "--",
                AutoSize = true,
                Size = new Size(40, 25),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Consolas", 12F, FontStyle.Regular, GraphicsUnit.Point, 0),
                Location = new Point(pnlOperator.Width - 30, 0),
                BorderStyle = BorderStyle.None,
            };
            pnlOperator.Controls.Add(lblTimer);
            Button btnImport = new Button
            {
                Text = "Import",
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Size = new Size(60, 25),
                Location = new Point(0, 30),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 1, MouseDownBackColor = Color.Gray },
            };
            btnImport.Click += (s, e) =>
            {
                var menu = new ContextMenuStrip();
                menu.Items.Add("匯入代碼", null, (s, ev) => ShowImportCodeDialog());
                menu.Items.Add("手動輸入", null, (s, ev) => ShowManualInputDialog());
                menu.Show(Cursor.Position);
            };
            pnlOperator.Controls.Add(btnImport);
            Timer timer = new Timer
            {
                Interval = 1000 // 1000ms
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            var timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            var stepSeconds = timestamp % 30; // 0 ~ 29
            lblTimer.Text = $"{30 - stepSeconds}";
            pnlContent.Controls.OfType<TOTPView>().ToList().ForEach(view =>
            {
                view.Ontick(timestamp);
            });
        }

        #region Dialog
        private void ShowImportCodeDialog()
        {
            using var dialog = new ImportCodeDialog();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string importText = dialog.InputText;
                foreach (var line in importText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (line.StartsWith("otpauth://", StringComparison.OrdinalIgnoreCase))
                    {
                        // 解析 otpauth URI
                        var uri = new Uri(line);
                        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                        string secret = query["secret"];
                        string issuer = query["issuer"] ?? "";
                        string label = Uri.UnescapeDataString(uri.AbsolutePath.Trim('/'));
                        // label 格式通常為 issuer:account
                        if (string.IsNullOrEmpty(issuer) && label.Contains(":"))
                            issuer = label.Split(':')[0];

                        if (!string.IsNullOrEmpty(secret))
                        {
                            RecordProvider.AddRecord(issuer, secret);
                        }
                    }
                }
                InitializeContent();
            }
        }

        private void ShowManualInputDialog()
        {
            using var dialog = new ManualInputDialog();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                string secret = dialog.Secret;
                string description = dialog.Description;
                RecordProvider.AddRecord(description, secret);
                InitializeContent();
            }
        } 
        #endregion

        public override void InitializeContent()
        {
            var records = RecordProvider.LoadRecords();
            ClearContent();
            foreach (var record in records)
            {
                try
                {
                    TOTPView toptView = new TOTPView(record.Secret, record.Description);
                    AddContent(toptView);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            base.InitializeContent();
        }
    }
}
