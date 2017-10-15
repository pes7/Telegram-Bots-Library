namespace GuchiBot
{
    partial class Main
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
            this.components = new System.ComponentModel.Container();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TimePlus = new System.Windows.Forms.Button();
            this.TimeStop = new System.Windows.Forms.Button();
            this.TimeMinus = new System.Windows.Forms.Button();
            this.TimeStart = new System.Windows.Forms.Button();
            this.TimePause = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(198, 20);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "WebmDir";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(3, 32);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(198, 20);
            this.textBox2.TabIndex = 3;
            this.textBox2.Text = "ImageDir";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(3, 61);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(198, 20);
            this.textBox3.TabIndex = 3;
            this.textBox3.Text = "PreViewDir";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.textBox3);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(207, 88);
            this.panel1.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.TimePlus);
            this.panel2.Controls.Add(this.TimeStop);
            this.panel2.Controls.Add(this.TimeMinus);
            this.panel2.Controls.Add(this.TimeStart);
            this.panel2.Controls.Add(this.TimePause);
            this.panel2.Location = new System.Drawing.Point(216, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(99, 88);
            this.panel2.TabIndex = 5;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Red;
            this.pictureBox1.Location = new System.Drawing.Point(35, 32);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(26, 23);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "20000 ms";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // TimePlus
            // 
            this.TimePlus.Location = new System.Drawing.Point(67, 32);
            this.TimePlus.Name = "TimePlus";
            this.TimePlus.Size = new System.Drawing.Size(26, 23);
            this.TimePlus.TabIndex = 0;
            this.TimePlus.Text = "+";
            this.TimePlus.UseVisualStyleBackColor = true;
            this.TimePlus.Click += new System.EventHandler(this.TimePlus_Click);
            // 
            // TimeStop
            // 
            this.TimeStop.Location = new System.Drawing.Point(67, 59);
            this.TimeStop.Name = "TimeStop";
            this.TimeStop.Size = new System.Drawing.Size(26, 23);
            this.TimeStop.TabIndex = 0;
            this.TimeStop.Text = "E";
            this.TimeStop.UseVisualStyleBackColor = true;
            this.TimeStop.Click += new System.EventHandler(this.TimeStop_Click);
            // 
            // TimeMinus
            // 
            this.TimeMinus.Location = new System.Drawing.Point(3, 32);
            this.TimeMinus.Name = "TimeMinus";
            this.TimeMinus.Size = new System.Drawing.Size(26, 23);
            this.TimeMinus.TabIndex = 0;
            this.TimeMinus.Text = "-";
            this.TimeMinus.UseVisualStyleBackColor = true;
            this.TimeMinus.Click += new System.EventHandler(this.TimeMinus_Click);
            // 
            // TimeStart
            // 
            this.TimeStart.Location = new System.Drawing.Point(3, 59);
            this.TimeStart.Name = "TimeStart";
            this.TimeStart.Size = new System.Drawing.Size(26, 23);
            this.TimeStart.TabIndex = 0;
            this.TimeStart.Text = "S";
            this.TimeStart.UseVisualStyleBackColor = true;
            this.TimeStart.Click += new System.EventHandler(this.TimeStart_Click);
            // 
            // TimePause
            // 
            this.TimePause.Location = new System.Drawing.Point(35, 59);
            this.TimePause.Name = "TimePause";
            this.TimePause.Size = new System.Drawing.Size(26, 23);
            this.TimePause.TabIndex = 0;
            this.TimePause.Text = "P";
            this.TimePause.UseVisualStyleBackColor = true;
            this.TimePause.Click += new System.EventHandler(this.TimePause_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(442, 247);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 1, 1);
            this.Text = "Гачи Бот";
            this.Load += new System.EventHandler(this.Main_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button TimeStop;
        private System.Windows.Forms.Button TimeStart;
        private System.Windows.Forms.Button TimePause;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button TimePlus;
        private System.Windows.Forms.Button TimeMinus;
        private System.Windows.Forms.Timer timer1;
    }
}