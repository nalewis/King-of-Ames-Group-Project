namespace LoginScreenWinForm
{
    partial class ServerListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serverList = new System.Windows.Forms.ListView();
            this.refresh = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // serverList
            // 
            this.serverList.Activation = System.Windows.Forms.ItemActivation.TwoClick;
            this.serverList.Location = new System.Drawing.Point(12, 12);
            this.serverList.Name = "serverList";
            this.serverList.Size = new System.Drawing.Size(260, 194);
            this.serverList.TabIndex = 1;
            this.serverList.UseCompatibleStateImageBehavior = false;
            this.serverList.View = System.Windows.Forms.View.List;
            // 
            // refresh
            // 
            this.refresh.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.refresh.Location = new System.Drawing.Point(146, 212);
            this.refresh.Name = "refresh";
            this.refresh.Size = new System.Drawing.Size(112, 23);
            this.refresh.TabIndex = 3;
            this.refresh.Text = "Main Menu";
            this.refresh.UseVisualStyleBackColor = true;
            this.refresh.Click += new System.EventHandler(this.leaveGame_Click);
            // 
            // button1
            // 
            this.button1.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button1.Location = new System.Drawing.Point(28, 212);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // ServerListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.refresh);
            this.Controls.Add(this.serverList);
            this.Name = "ServerListForm";
            this.Text = "ServerList";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView serverList;
        private System.Windows.Forms.Button refresh;
        private System.Windows.Forms.Button button1;
    }
}