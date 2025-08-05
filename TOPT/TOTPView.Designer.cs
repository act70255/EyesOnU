namespace TOTP
{
    partial class TOTPView
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            //progressBar = new ProgressBar();
            labelTotp = new Label();
            labelDescription = new Label();
            //labelTime = new Label();
            btnDelete = new Button();
            SuspendLayout();
            //// 
            //// progressBar
            //// 
            //progressBar.Dock = DockStyle.Bottom;
            //progressBar.Height = 10;
            //progressBar.Minimum = 0;
            //progressBar.Maximum = 30;
            //progressBar.Value = 0;
            //
            // labelTotp
            //
            labelTotp.Dock = DockStyle.Fill;
            labelTotp.Font = new Font("Consolas", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelTotp.TextAlign = ContentAlignment.MiddleCenter;
            labelTotp.Text = "------";
            //labelTotp.Margin = new Padding(0, 0, 0, -10);
            //labelTotp.Padding = new Padding(0, 0, 0, -10);
            ////
            //// labelTime
            ////
            //labelTime.Font = new Font("Consolas", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            //labelTime.TextAlign = ContentAlignment.MiddleCenter;
            //labelTime.Text = "00:00";
            //labelTime.Margin = new Padding(0);
            //labelTime.AutoSize = false;
            //labelTime.Size = new Size(40, 25);
            //labelTime.Location = new Point(400, 10);
            //
            // labelDescription
            //
            labelDescription.Dock = DockStyle.Top;
            labelDescription.Font = new Font("Consolas", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            labelDescription.Height = 10;
            labelDescription.TextAlign = ContentAlignment.MiddleLeft;
            labelDescription.Text = "TOTP Code";
            labelDescription.Location = new Point(0, 0);
            labelDescription.Margin = new Padding(0);
            labelDescription.Padding = new Padding(0);
            labelDescription.Height = 25;
            //
            // btnDelete
            //
            btnDelete.Text = "❎";
            btnDelete.Size = new Size(30, 30);
            btnDelete.Padding = new Padding(0, 0, 0, 0);
            btnDelete.Margin = new Padding(0, 0, 0, 0);
            btnDelete.TextAlign = ContentAlignment.MiddleCenter;
            btnDelete.Font = new Font("Consolas", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnDelete.Location = new Point(210, 0);
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.FlatAppearance.BorderSize = 0;

            //this.Controls.Add(labelTime);
            this.Controls.Add(btnDelete);
            this.Controls.Add(labelTotp);
            this.Controls.Add(labelDescription);
            //this.Controls.Add(progressBar);
            this.Margin = new Padding(0);
            this.Padding = new Padding(0);
            this.Width = 240;
            this.Height = 80;
            BackColor = Color.FromArgb(30, 30, 30);
            this.MouseUp += TOTPView_MouseUp;

            ResumeLayout();
        }

        private Label labelTotp;
        private Label labelDescription;
        private Button btnDelete;
        #endregion
    }
}
