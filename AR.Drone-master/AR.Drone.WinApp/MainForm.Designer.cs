namespace AR.Drone.WinApp
{
    partial class MainForm
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
            this.pbVideo = new System.Windows.Forms.PictureBox();
            this.tmrStateUpdate = new System.Windows.Forms.Timer(this.components);
            this.tvInfo = new System.Windows.Forms.TreeView();
            this.tmrVideoUpdate = new System.Windows.Forms.Timer(this.components);
            this.btnStopRecording = new System.Windows.Forms.Button();
            this.btnReplay = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbVideo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pbVideo
            // 
            this.pbVideo.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pbVideo.Location = new System.Drawing.Point(13, 50);
            this.pbVideo.Margin = new System.Windows.Forms.Padding(4);
            this.pbVideo.Name = "pbVideo";
            this.pbVideo.Size = new System.Drawing.Size(1344, 653);
            this.pbVideo.TabIndex = 2;
            this.pbVideo.TabStop = false;
            // 
            // tmrStateUpdate
            // 
            this.tmrStateUpdate.Interval = 500;
            this.tmrStateUpdate.Tick += new System.EventHandler(this.tmrStateUpdate_Tick);
            // 
            // tvInfo
            // 
            this.tvInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvInfo.Location = new System.Drawing.Point(1375, 50);
            this.tvInfo.Margin = new System.Windows.Forms.Padding(4);
            this.tvInfo.Name = "tvInfo";
            this.tvInfo.Size = new System.Drawing.Size(331, 497);
            this.tvInfo.TabIndex = 18;
            // 
            // tmrVideoUpdate
            // 
            this.tmrVideoUpdate.Interval = 20;
            this.tmrVideoUpdate.Tick += new System.EventHandler(this.tmrVideoUpdate_Tick);
            // 
            // btnStopRecording
            // 
            this.btnStopRecording.Location = new System.Drawing.Point(1471, 726);
            this.btnStopRecording.Margin = new System.Windows.Forms.Padding(4);
            this.btnStopRecording.Name = "btnStopRecording";
            this.btnStopRecording.Size = new System.Drawing.Size(100, 28);
            this.btnStopRecording.TabIndex = 23;
            this.btnStopRecording.Text = "Stop Rec.";
            this.btnStopRecording.UseVisualStyleBackColor = true;
            this.btnStopRecording.Click += new System.EventHandler(this.btnStopRecording_Click);
            // 
            // btnReplay
            // 
            this.btnReplay.Location = new System.Drawing.Point(1364, 756);
            this.btnReplay.Margin = new System.Windows.Forms.Padding(4);
            this.btnReplay.Name = "btnReplay";
            this.btnReplay.Size = new System.Drawing.Size(207, 28);
            this.btnReplay.TabIndex = 24;
            this.btnReplay.Text = "Play";
            this.btnReplay.UseVisualStyleBackColor = true;
            this.btnReplay.Click += new System.EventHandler(this.btnReplay_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1364, 726);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 28);
            this.button1.TabIndex = 25;
            this.button1.Text = "Get Image";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(13, 726);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1345, 201);
            this.pictureBox1.TabIndex = 26;
            this.pictureBox1.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1794, 939);
            this.Controls.Add(this.pbVideo);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnReplay);
            this.Controls.Add(this.btnStopRecording);
            this.Controls.Add(this.tvInfo);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "TTU ARDrone Contol Station";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Main_Form_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Main_Form_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pbVideo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbVideo;
        private System.Windows.Forms.Timer tmrStateUpdate;
        private System.Windows.Forms.TreeView tvInfo;
        private System.Windows.Forms.Timer tmrVideoUpdate;
        private System.Windows.Forms.Button btnStopRecording;
        private System.Windows.Forms.Button btnReplay;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

