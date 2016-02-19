namespace GolfBallTracking
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Disposable
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
        #endregion Disposable


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.camImageBox = new Emgu.CV.UI.ImageBox();
            this.btnLoadVideo = new System.Windows.Forms.Button();
            this.frameTimer = new System.Windows.Forms.Timer(this.components);
            this.btnStartTracking = new System.Windows.Forms.Button();
            this.frameSlider = new System.Windows.Forms.TrackBar();
            this.btnPlayPauseVideo = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.nudTemplateSize = new System.Windows.Forms.NumericUpDown();
            this.btnClearBgr = new System.Windows.Forms.Button();
            this.pbTemplateImage = new System.Windows.Forms.PictureBox();
            this.pbBall = new System.Windows.Forms.PictureBox();
            this.nudValue = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.camImageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTemplateSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTemplateImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBall)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudValue)).BeginInit();
            this.SuspendLayout();
            // 
            // camImageBox
            // 
            this.camImageBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.camImageBox.Location = new System.Drawing.Point(13, 13);
            this.camImageBox.Name = "camImageBox";
            this.camImageBox.Size = new System.Drawing.Size(640, 480);
            this.camImageBox.TabIndex = 2;
            this.camImageBox.TabStop = false;
            // 
            // btnLoadVideo
            // 
            this.btnLoadVideo.Location = new System.Drawing.Point(677, 43);
            this.btnLoadVideo.Name = "btnLoadVideo";
            this.btnLoadVideo.Size = new System.Drawing.Size(115, 23);
            this.btnLoadVideo.TabIndex = 4;
            this.btnLoadVideo.Text = "Load Video";
            this.btnLoadVideo.UseVisualStyleBackColor = true;
            this.btnLoadVideo.Click += new System.EventHandler(this.btnLoadVideo_Click);
            // 
            // frameTimer
            // 
            this.frameTimer.Tick += new System.EventHandler(this.frameTimer_Tick);
            // 
            // btnStartTracking
            // 
            this.btnStartTracking.Location = new System.Drawing.Point(677, 72);
            this.btnStartTracking.Name = "btnStartTracking";
            this.btnStartTracking.Size = new System.Drawing.Size(115, 23);
            this.btnStartTracking.TabIndex = 5;
            this.btnStartTracking.Text = "StartTracking";
            this.btnStartTracking.UseVisualStyleBackColor = true;
            this.btnStartTracking.Click += new System.EventHandler(this.btnStartTracking_Click);
            // 
            // frameSlider
            // 
            this.frameSlider.Location = new System.Drawing.Point(13, 505);
            this.frameSlider.Name = "frameSlider";
            this.frameSlider.Size = new System.Drawing.Size(640, 45);
            this.frameSlider.TabIndex = 6;
            this.frameSlider.ValueChanged += new System.EventHandler(this.frameSlider_ValueChanged);
            // 
            // btnPlayPauseVideo
            // 
            this.btnPlayPauseVideo.Enabled = false;
            this.btnPlayPauseVideo.Location = new System.Drawing.Point(12, 540);
            this.btnPlayPauseVideo.Name = "btnPlayPauseVideo";
            this.btnPlayPauseVideo.Size = new System.Drawing.Size(75, 23);
            this.btnPlayPauseVideo.TabIndex = 7;
            this.btnPlayPauseVideo.Text = "Play/Pause";
            this.btnPlayPauseVideo.UseVisualStyleBackColor = true;
            this.btnPlayPauseVideo.Click += new System.EventHandler(this.btnPlayPauseVideo_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(730, 153);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Template size";
            // 
            // nudTemplateSize
            // 
            this.nudTemplateSize.Enabled = false;
            this.nudTemplateSize.Location = new System.Drawing.Point(677, 151);
            this.nudTemplateSize.Maximum = new decimal(new int[] {
            35,
            0,
            0,
            0});
            this.nudTemplateSize.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudTemplateSize.Name = "nudTemplateSize";
            this.nudTemplateSize.Size = new System.Drawing.Size(47, 20);
            this.nudTemplateSize.TabIndex = 9;
            this.nudTemplateSize.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.nudTemplateSize.ValueChanged += new System.EventHandler(this.nudTemplateSize_ValueChanged);
            // 
            // btnClearBgr
            // 
            this.btnClearBgr.Enabled = false;
            this.btnClearBgr.Location = new System.Drawing.Point(677, 262);
            this.btnClearBgr.Name = "btnClearBgr";
            this.btnClearBgr.Size = new System.Drawing.Size(115, 23);
            this.btnClearBgr.TabIndex = 10;
            this.btnClearBgr.Text = "Clear all tracks";
            this.btnClearBgr.UseVisualStyleBackColor = true;
            this.btnClearBgr.Click += new System.EventHandler(this.btnDetectBall_Click);
            // 
            // pbTemplateImage
            // 
            this.pbTemplateImage.Location = new System.Drawing.Point(677, 195);
            this.pbTemplateImage.Name = "pbTemplateImage";
            this.pbTemplateImage.Size = new System.Drawing.Size(40, 40);
            this.pbTemplateImage.TabIndex = 11;
            this.pbTemplateImage.TabStop = false;
            // 
            // pbBall
            // 
            this.pbBall.Location = new System.Drawing.Point(830, 72);
            this.pbBall.Name = "pbBall";
            this.pbBall.Size = new System.Drawing.Size(191, 186);
            this.pbBall.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbBall.TabIndex = 12;
            this.pbBall.TabStop = false;
            // 
            // nudValue
            // 
            this.nudValue.Enabled = false;
            this.nudValue.Location = new System.Drawing.Point(678, 119);
            this.nudValue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudValue.Name = "nudValue";
            this.nudValue.Size = new System.Drawing.Size(46, 20);
            this.nudValue.TabIndex = 14;
            this.nudValue.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.nudValue.ValueChanged += new System.EventHandler(this.nudValue_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(730, 121);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Threshold value";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1050, 575);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nudValue);
            this.Controls.Add(this.pbBall);
            this.Controls.Add(this.pbTemplateImage);
            this.Controls.Add(this.btnClearBgr);
            this.Controls.Add(this.nudTemplateSize);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnPlayPauseVideo);
            this.Controls.Add(this.frameSlider);
            this.Controls.Add(this.btnStartTracking);
            this.Controls.Add(this.btnLoadVideo);
            this.Controls.Add(this.camImageBox);
            this.Name = "MainForm";
            this.Text = "Golf Ball Tracking";
            ((System.ComponentModel.ISupportInitialize)(this.camImageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTemplateSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTemplateImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBall)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        private Emgu.CV.UI.ImageBox camImageBox;
        private System.Windows.Forms.Button btnLoadVideo;
        private System.Windows.Forms.Timer frameTimer;
        private System.Windows.Forms.Button btnStartTracking;
        private System.Windows.Forms.TrackBar frameSlider;
        private System.Windows.Forms.Button btnPlayPauseVideo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudTemplateSize;
        private System.Windows.Forms.Button btnClearBgr;
        private System.Windows.Forms.PictureBox pbTemplateImage;
        private System.Windows.Forms.PictureBox pbBall;
        private System.Windows.Forms.NumericUpDown nudValue;
        private System.Windows.Forms.Label label2;
    }
}

