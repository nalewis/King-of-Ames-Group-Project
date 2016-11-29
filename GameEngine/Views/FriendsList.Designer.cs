namespace GameEngine.Views
{
    partial class FriendsList
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.boxOfriends = new System.Windows.Forms.ListView();
            this.mainMenu = new System.Windows.Forms.Button();
            this.addFriend = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.boxOfriends);
            this.groupBox1.Location = new System.Drawing.Point(10, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(262, 237);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Friends";
            // 
            // boxOfriends
            // 
            this.boxOfriends.Location = new System.Drawing.Point(15, 19);
            this.boxOfriends.Name = "boxOfriends";
            this.boxOfriends.Size = new System.Drawing.Size(232, 202);
            this.boxOfriends.TabIndex = 0;
            this.boxOfriends.UseCompatibleStateImageBehavior = false;
            this.boxOfriends.View = System.Windows.Forms.View.List;
            // 
            // mainMenu
            // 
            this.mainMenu.Location = new System.Drawing.Point(25, 255);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(75, 23);
            this.mainMenu.TabIndex = 1;
            this.mainMenu.Text = "Main Menu";
            this.mainMenu.UseVisualStyleBackColor = true;
            this.mainMenu.Click += new System.EventHandler(this.mainMenu_Click);
            // 
            // addFriend
            // 
            this.addFriend.Location = new System.Drawing.Point(182, 255);
            this.addFriend.Name = "addFriend";
            this.addFriend.Size = new System.Drawing.Size(75, 23);
            this.addFriend.TabIndex = 2;
            this.addFriend.Text = "Add Friend";
            this.addFriend.UseVisualStyleBackColor = true;
            this.addFriend.Click += new System.EventHandler(this.addFriend_Click);
            // 
            // FriendsList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Chocolate;
            this.ClientSize = new System.Drawing.Size(284, 285);
            this.Controls.Add(this.addFriend);
            this.Controls.Add(this.mainMenu);
            this.Controls.Add(this.groupBox1);
            this.Name = "FriendsList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "King of Ames";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView boxOfriends;
        private System.Windows.Forms.Button mainMenu;
        private System.Windows.Forms.Button addFriend;
    }
}