namespace LoginScreenWinForm
{
    partial class HostGameListForm
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
            this.playerList = new System.Windows.Forms.ListView();
            this.leaveGame = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // playerList
            // 
            this.playerList.Location = new System.Drawing.Point(12, 12);
            this.playerList.Name = "playerList";
            this.playerList.Size = new System.Drawing.Size(260, 194);
            this.playerList.TabIndex = 0;
            this.playerList.UseCompatibleStateImageBehavior = false;    
            // 
            // leaveGame
            // 
            this.leaveGame.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.leaveGame.Location = new System.Drawing.Point(83, 212);
            this.leaveGame.Name = "leaveGame";
            this.leaveGame.Size = new System.Drawing.Size(112, 23);
            this.leaveGame.TabIndex = 2;
            this.leaveGame.Text = "Leave Game";
            this.leaveGame.UseVisualStyleBackColor = true;
            this.leaveGame.Click += new System.EventHandler(this.leaveGame_Click);
            // 
            // HostGameListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.leaveGame);
            this.Controls.Add(this.playerList);
            this.Name = "HostGameListForm";
            this.Text = "ServerListForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView playerList;
        private System.Windows.Forms.Button leaveGame;
    }
}