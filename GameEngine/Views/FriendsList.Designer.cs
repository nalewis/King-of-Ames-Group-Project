using System;
using System.Windows.Forms;

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
            this.BoxOFriends = new System.Windows.Forms.ListView();
            this.playerName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.addFriend = new System.Windows.Forms.Button();
            this.delFriend = new System.Windows.Forms.Button();
            this.Location = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BoxOFriends);
            this.groupBox1.Location = new System.Drawing.Point(10, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(262, 237);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Friends";
            // 
            // BoxOFriends
            // 
            this.BoxOFriends.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.playerName,
            this.status,
            this.Location});
            this.BoxOFriends.FullRowSelect = true;
            this.BoxOFriends.GridLines = true;
            this.BoxOFriends.Location = new System.Drawing.Point(15, 19);
            this.BoxOFriends.Name = "BoxOFriends";
            this.BoxOFriends.Size = new System.Drawing.Size(232, 202);
            this.BoxOFriends.TabIndex = 1;
            this.BoxOFriends.UseCompatibleStateImageBehavior = false;
            this.BoxOFriends.View = System.Windows.Forms.View.Details;
            this.BoxOFriends.SelectedIndexChanged += new System.EventHandler(this.BoxOFriends_SelectedIndexChanged);
            // 
            // playerName
            // 
            this.playerName.Text = "Player Name";
            this.playerName.Width = 89;
            // 
            // status
            // 
            this.status.Text = "Status";
            this.status.Width = 66;
            // 
            // addFriend
            // 
            this.addFriend.Location = new System.Drawing.Point(25, 255);
            this.addFriend.Name = "addFriend";
            this.addFriend.Size = new System.Drawing.Size(83, 23);
            this.addFriend.TabIndex = 2;
            this.addFriend.Text = "Add Friend";
            this.addFriend.UseVisualStyleBackColor = true;
            this.addFriend.Click += new System.EventHandler(this.addFriend_Click);
            // 
            // delFriend
            // 
            this.delFriend.Location = new System.Drawing.Point(179, 255);
            this.delFriend.Name = "delFriend";
            this.delFriend.Size = new System.Drawing.Size(78, 23);
            this.delFriend.TabIndex = 3;
            this.delFriend.Text = "Delete Friend";
            this.delFriend.UseVisualStyleBackColor = true;
            this.delFriend.Click += new System.EventHandler(this.delFriend_Click);
            // 
            // Location
            // 
            this.Location.Text = "Location";
            // 
            // FriendsList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Chocolate;
            this.ClientSize = new System.Drawing.Size(284, 285);
            this.Controls.Add(this.delFriend);
            this.Controls.Add(this.addFriend);
            this.Controls.Add(this.groupBox1);
            this.Name = "FriendsList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "King of Ames";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FriendsList_FormClosing);
            this.Disposed += new System.EventHandler(this.FriendsList_Disposed);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button addFriend;
        private System.Windows.Forms.Button delFriend;
        private ListView BoxOFriends;
        private ColumnHeader playerName;
        private ColumnHeader status;
        private ColumnHeader Location;
    }
}