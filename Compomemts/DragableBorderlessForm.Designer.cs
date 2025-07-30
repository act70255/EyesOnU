namespace TOPT
{
    partial class DragableBorderlessForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pnlOperator = new Panel();
            chkLock = new CheckBox();
            btnExit = new Button();
            lblMessage = new Label();
            pnlContent = new Panel();
            pnlOperator.SuspendLayout();
            SuspendLayout();
            // 
            // pnlOperator
            // 
            pnlOperator.AutoSize = true;
            pnlOperator.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlOperator.BorderStyle = BorderStyle.FixedSingle;
            pnlOperator.Controls.Add(chkLock);
            pnlOperator.Controls.Add(btnExit);
            pnlOperator.Controls.Add(lblMessage);
            pnlOperator.Dock = DockStyle.Top;
            pnlOperator.Location = new Point(0, 0);
            pnlOperator.Margin = new Padding(5);
            pnlOperator.Name = "pnlOperator";
            pnlOperator.Size = new Size(400, 58);
            // 
            // chkLock
            // 
            chkLock.AutoSize = true;
            chkLock.Location = new Point(10, 0);
            chkLock.Name = "chkLock";
            chkLock.Size = new Size(126, 27);
            chkLock.TabIndex = 0;
            chkLock.Text = "維持在上面";
            chkLock.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnExit
            // 
            btnExit.Dock = DockStyle.Right;
            btnExit.Location = new Point(338, 0);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(60, 56);
            btnExit.TabIndex = 1;
            btnExit.Text = "Exit";
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.FlatAppearance.BorderSize = 1;
            btnExit.FlatAppearance.MouseDownBackColor = Color.Gray;
            // 
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.Font = new Font("Microsoft Sans Serif", 12F);
            lblMessage.Location = new Point(5, 27);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 29);
            lblMessage.TabIndex = 2;
            // 
            // pnlContent
            // 
            pnlContent.AutoSize = true;
            pnlContent.BorderStyle = BorderStyle.FixedSingle;
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Location = new Point(0, 58);
            pnlContent.Name = "pnlContent";
            pnlContent.Padding = new Padding(5, 5, 5, 5);
            //pnlContent.Size = new Size(300, 207);
            pnlContent.TabIndex = 0;
            // 
            // OPTForm
            // 
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            //ClientSize = new Size(400, 265);
            MinimumSize = new Size(300, 58);
            Controls.Add(pnlContent);
            Controls.Add(pnlOperator);
            Name = "OPTForm";
            Text = "TOPT";
            pnlOperator.ResumeLayout(false);
            pnlOperator.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        protected Panel pnlOperator;
        protected Panel pnlContent;
        protected Label lblMessage;
        protected CheckBox chkLock;
        protected Button btnExit;
    }
}
