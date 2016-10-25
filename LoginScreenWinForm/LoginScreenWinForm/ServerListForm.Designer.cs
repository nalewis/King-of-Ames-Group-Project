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
            this.mainMenu = new System.Windows.Forms.Button();
            this.join = new System.Windows.Forms.Button();
            this.serverList = new System.Windows.Forms.ListBox();
            this.refresh = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.mainMenu.Location = new System.Drawing.Point(146, 203);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(112, 23);
            this.mainMenu.TabIndex = 3;
            this.mainMenu.Text = "Main Menu";
            this.mainMenu.UseVisualStyleBackColor = true;
            this.mainMenu.Click += new System.EventHandler(this.mainMenu_Click);
            // 
            // join
            // 
            this.join.BackColor = System.Drawing.SystemColors.ControlDark;
            this.join.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.join.Location = new System.Drawing.Point(28, 232);
            this.join.Name = "join";
            this.join.Size = new System.Drawing.Size(230, 28);
            this.join.TabIndex = 5;
            this.join.Text = "Join Game";
            this.join.UseVisualStyleBackColor = false;
            this.join.Click += new System.EventHandler(this.join_Click);
            // 
            // serverList
            // 
            this.serverList.FormattingEnabled = true;
            this.serverList.Location = new System.Drawing.Point(28, 12);
            this.serverList.Name = "serverList";
            this.serverList.Size = new System.Drawing.Size(230, 173);
            this.serverList.TabIndex = 6;
            this.serverList.Click += new System.EventHandler(this.serverList_Click);
            // 
            // refresh
            // 
            this.refresh.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.refresh.Location = new System.Drawing.Point(28, 203);
            this.refresh.Name = "refresh";
            this.refresh.Size = new System.Drawing.Size(112, 23);
            this.refresh.TabIndex = 7;
            this.refresh.Text = "Refresh";
            this.refresh.UseVisualStyleBackColor = true;
            this.refresh.Click += new System.EventHandler(this.refresh_Click);
            // 
            // ServerListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.refresh);
            this.Controls.Add(this.serverList);
            this.Controls.Add(this.join);
            this.Controls.Add(this.mainMenu);
            this.Name = "ServerListForm";
            this.Text = "ServerList";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button mainMenu;
        private System.Windows.Forms.Button join;
        private System.Windows.Forms.ListBox serverList;
        private System.Windows.Forms.Button refresh;
    }
}