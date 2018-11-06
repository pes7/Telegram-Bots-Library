namespace Pic_finder
{
    partial class Ctr_panel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ctr_panel));
            this.Msg_type_group = new System.Windows.Forms.GroupBox();
            this.radioFile = new System.Windows.Forms.RadioButton();
            this.radioVideo = new System.Windows.Forms.RadioButton();
            this.radioPhoto = new System.Windows.Forms.RadioButton();
            this.radioText = new System.Windows.Forms.RadioButton();
            this.MsgText = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioDelete = new System.Windows.Forms.RadioButton();
            this.radioReply = new System.Windows.Forms.RadioButton();
            this.radioSend = new System.Windows.Forms.RadioButton();
            this.MsgIdText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ChatIdEdit = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.MsgLog = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.UsersMsgs = new System.Windows.Forms.TextBox();
            this.RefMsgs = new System.Windows.Forms.Button();
            this.Msg_type_group.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Msg_type_group
            // 
            this.Msg_type_group.Controls.Add(this.radioFile);
            this.Msg_type_group.Controls.Add(this.radioVideo);
            this.Msg_type_group.Controls.Add(this.radioPhoto);
            this.Msg_type_group.Controls.Add(this.radioText);
            this.Msg_type_group.Location = new System.Drawing.Point(393, 12);
            this.Msg_type_group.Name = "Msg_type_group";
            this.Msg_type_group.Size = new System.Drawing.Size(92, 117);
            this.Msg_type_group.TabIndex = 0;
            this.Msg_type_group.TabStop = false;
            this.Msg_type_group.Text = "Message type";
            // 
            // radioFile
            // 
            this.radioFile.AutoSize = true;
            this.radioFile.Location = new System.Drawing.Point(6, 89);
            this.radioFile.Name = "radioFile";
            this.radioFile.Size = new System.Drawing.Size(41, 17);
            this.radioFile.TabIndex = 10;
            this.radioFile.Text = "File";
            this.radioFile.UseVisualStyleBackColor = true;
            this.radioFile.CheckedChanged += new System.EventHandler(this.radioPhoto_CheckedChanged);
            // 
            // radioVideo
            // 
            this.radioVideo.AutoSize = true;
            this.radioVideo.Location = new System.Drawing.Point(7, 66);
            this.radioVideo.Name = "radioVideo";
            this.radioVideo.Size = new System.Drawing.Size(52, 17);
            this.radioVideo.TabIndex = 9;
            this.radioVideo.Text = "Video";
            this.radioVideo.UseVisualStyleBackColor = true;
            this.radioVideo.CheckedChanged += new System.EventHandler(this.radioPhoto_CheckedChanged);
            // 
            // radioPhoto
            // 
            this.radioPhoto.AutoSize = true;
            this.radioPhoto.Location = new System.Drawing.Point(7, 43);
            this.radioPhoto.Name = "radioPhoto";
            this.radioPhoto.Size = new System.Drawing.Size(53, 17);
            this.radioPhoto.TabIndex = 8;
            this.radioPhoto.Text = "Photo";
            this.radioPhoto.UseVisualStyleBackColor = true;
            this.radioPhoto.CheckedChanged += new System.EventHandler(this.radioPhoto_CheckedChanged);
            // 
            // radioText
            // 
            this.radioText.AutoSize = true;
            this.radioText.Checked = true;
            this.radioText.Location = new System.Drawing.Point(7, 20);
            this.radioText.Name = "radioText";
            this.radioText.Size = new System.Drawing.Size(46, 17);
            this.radioText.TabIndex = 7;
            this.radioText.TabStop = true;
            this.radioText.Text = "Text";
            this.radioText.UseVisualStyleBackColor = true;
            // 
            // MsgText
            // 
            this.MsgText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.MsgText.Location = new System.Drawing.Point(393, 190);
            this.MsgText.Multiline = true;
            this.MsgText.Name = "MsgText";
            this.MsgText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.MsgText.Size = new System.Drawing.Size(187, 158);
            this.MsgText.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(393, 354);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(187, 25);
            this.button1.TabIndex = 4;
            this.button1.Text = "Initiate";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioDelete);
            this.groupBox1.Controls.Add(this.radioReply);
            this.groupBox1.Controls.Add(this.radioSend);
            this.groupBox1.Location = new System.Drawing.Point(495, 36);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(89, 93);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Operation";
            // 
            // radioDelete
            // 
            this.radioDelete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radioDelete.AutoSize = true;
            this.radioDelete.Location = new System.Drawing.Point(7, 66);
            this.radioDelete.Name = "radioDelete";
            this.radioDelete.Size = new System.Drawing.Size(56, 17);
            this.radioDelete.TabIndex = 6;
            this.radioDelete.Text = "Delete";
            this.radioDelete.UseVisualStyleBackColor = true;
            // 
            // radioReply
            // 
            this.radioReply.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radioReply.AutoSize = true;
            this.radioReply.Location = new System.Drawing.Point(7, 43);
            this.radioReply.Name = "radioReply";
            this.radioReply.Size = new System.Drawing.Size(52, 17);
            this.radioReply.TabIndex = 5;
            this.radioReply.Text = "Reply";
            this.radioReply.UseVisualStyleBackColor = true;
            // 
            // radioSend
            // 
            this.radioSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radioSend.AutoSize = true;
            this.radioSend.Checked = true;
            this.radioSend.Location = new System.Drawing.Point(7, 20);
            this.radioSend.Name = "radioSend";
            this.radioSend.Size = new System.Drawing.Size(50, 17);
            this.radioSend.TabIndex = 4;
            this.radioSend.TabStop = true;
            this.radioSend.Text = "Send";
            this.radioSend.UseVisualStyleBackColor = true;
            // 
            // MsgIdText
            // 
            this.MsgIdText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MsgIdText.Location = new System.Drawing.Point(5, 19);
            this.MsgIdText.Name = "MsgIdText";
            this.MsgIdText.Size = new System.Drawing.Size(84, 20);
            this.MsgIdText.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Message Id";
            // 
            // ChatIdEdit
            // 
            this.ChatIdEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChatIdEdit.Location = new System.Drawing.Point(95, 19);
            this.ChatIdEdit.Name = "ChatIdEdit";
            this.ChatIdEdit.Size = new System.Drawing.Size(86, 20);
            this.ChatIdEdit.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(92, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Chat Id";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.ChatIdEdit);
            this.groupBox2.Controls.Add(this.MsgIdText);
            this.groupBox2.Location = new System.Drawing.Point(393, 136);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(187, 48);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            // 
            // MsgLog
            // 
            this.MsgLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.MsgLog.Location = new System.Drawing.Point(13, 13);
            this.MsgLog.Multiline = true;
            this.MsgLog.Name = "MsgLog";
            this.MsgLog.ReadOnly = true;
            this.MsgLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.MsgLog.Size = new System.Drawing.Size(374, 365);
            this.MsgLog.TabIndex = 5;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Multiselect = true;
            // 
            // UsersMsgs
            // 
            this.UsersMsgs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UsersMsgs.Location = new System.Drawing.Point(590, 13);
            this.UsersMsgs.Multiline = true;
            this.UsersMsgs.Name = "UsersMsgs";
            this.UsersMsgs.ReadOnly = true;
            this.UsersMsgs.Size = new System.Drawing.Size(425, 365);
            this.UsersMsgs.TabIndex = 6;
            // 
            // RefMsgs
            // 
            this.RefMsgs.Location = new System.Drawing.Point(495, 6);
            this.RefMsgs.Name = "RefMsgs";
            this.RefMsgs.Size = new System.Drawing.Size(85, 24);
            this.RefMsgs.TabIndex = 7;
            this.RefMsgs.Text = "Refresh";
            this.RefMsgs.UseVisualStyleBackColor = true;
            this.RefMsgs.Click += new System.EventHandler(this.RefMsgs_Click);
            // 
            // Ctr_panel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1027, 390);
            this.Controls.Add(this.RefMsgs);
            this.Controls.Add(this.UsersMsgs);
            this.Controls.Add(this.MsgLog);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.MsgText);
            this.Controls.Add(this.Msg_type_group);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Ctr_panel";
            this.Text = "AniPic Send Panel";
            this.Load += new System.EventHandler(this.Ctr_panel_Load);
            this.Msg_type_group.ResumeLayout(false);
            this.Msg_type_group.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox Msg_type_group;
        private System.Windows.Forms.RadioButton radioFile;
        private System.Windows.Forms.RadioButton radioVideo;
        private System.Windows.Forms.RadioButton radioPhoto;
        private System.Windows.Forms.RadioButton radioText;
        private System.Windows.Forms.TextBox MsgText;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioDelete;
        private System.Windows.Forms.TextBox MsgIdText;
        private System.Windows.Forms.RadioButton radioReply;
        private System.Windows.Forms.RadioButton radioSend;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ChatIdEdit;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox MsgLog;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox UsersMsgs;
        private System.Windows.Forms.Button RefMsgs;
    }
}