using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Cvb;
using Emgu.CV.Features2D;

namespace GolfBallTracking
{
    public partial class MainForm : Form
    {
        private Capture capture;
        private int FPS;
        private int frameCount;
        private int currentFrameNumber;
        private bool tracking;
        private List<Mat> image_array = new List<Mat>();

        public MainForm()
        {
            InitializeComponent();
            FPS = 30;
            tracking = false;
            nudTemplateSize.Value = nudTemplateSize.Maximum;
            showTemplateInPictureBox();
        }

        private void ReleaseData()
        {
            if (capture != null)
                capture.Dispose();
        }

        private void btnLoadVideo_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            string formats = "All Videos Files |*.dat; *.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso; *.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; " +
                  " *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4; *.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm";
            ofd.Filter = formats;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string fileName = ofd.FileName;

                Console.WriteLine("Video path: " + fileName);

                capture = new Capture(fileName);

                FPS = (int)capture.GetCaptureProperty(CapProp.Fps);
                int frameWidth = (int)capture.GetCaptureProperty(CapProp.FrameWidth);
                int frameHeight = (int)capture.GetCaptureProperty(CapProp.FrameHeight);
                frameCount = (int)capture.GetCaptureProperty(CapProp.FrameCount);
            
                Console.WriteLine("FPS: " + FPS);
                Console.WriteLine("FrameWidth: " + frameWidth);
                Console.WriteLine("FrameHeight: " + frameHeight);
                Console.WriteLine("FrameCount: " + frameCount);

                if (frameWidth < 680)
                {
                    camImageBox.Width = frameWidth;
                    frameSlider.Width = frameWidth;
                }
                if (frameHeight < 480)
                    camImageBox.Height = frameHeight;


                currentFrameNumber = 0;
                frameSlider.Maximum = frameCount;
                btnPlayPauseVideo.Text = "Pause";
                btnPlayPauseVideo.Enabled = true;

                //Frame rate
                frameTimer.Interval = 1000 / FPS;
                frameTimer.Start();
            }
        }

        private void frameTimer_Tick(object sender, EventArgs e)
        {           
            Mat frame = capture.QueryFrame();
            if (frame != null || currentFrameNumber >= frameCount)
            {
                camImageBox.Image = frame;
                if (tracking)
                {
                    if (image_array.Count == 20)
                        image_array.RemoveAt(0);
                    image_array.Add(frame);
                    BallDetection.Instance.Detect(this, image_array.Last(), (int)nudValue.Value);
                }

                frameSlider.Value = currentFrameNumber++;
            }
            else
            {
                frameTimer.Stop();
            }

        }

        private void btnStartTracking_Click(object sender, EventArgs e)
        {
            tracking = true; 
        }

        private void btnPlayPauseVideo_Click(object sender, EventArgs e)
        {
            if (btnPlayPauseVideo.Text == "Pause")
            {
                frameTimer.Stop();
                btnPlayPauseVideo.Text = "Play";
            }
            else
            {
                frameTimer.Start();
                btnPlayPauseVideo.Text = "Pause";
            }
        }

        private void frameSlider_ValueChanged(object sender, EventArgs e)
        {
            currentFrameNumber = frameSlider.Value;
            image_array.Clear();
            capture.SetCaptureProperty(CapProp.PosFrames, currentFrameNumber);
        }

        private void nudTemplateSize_ValueChanged(object sender, EventArgs e)
        {
            BallDetection.Instance.TemplateSize = (int)nudTemplateSize.Value;
            pbTemplateImage.Width = (int)nudTemplateSize.Value;
            pbTemplateImage.Height = (int)nudTemplateSize.Value;
            showTemplateInPictureBox();
        }

        private void showTemplateInPictureBox() 
        {
            pbTemplateImage.Image = BallDetection.Instance.TemplateImage.Bitmap;
        }

        private void btnDetectBall_Click(object sender, EventArgs e)
        {
            BallDetection.Instance.Detect(this, image_array.Last(), (int)nudValue.Value);
        }

        public void showGrayImage(Image<Gray, Byte> image) 
        {
            pbBall.Image = image.Bitmap;
        }

        public void showGrayImageThreshold(Image<Gray, Byte> image)
        {
            pictureBox1.Image = image.Bitmap;
        }

        private void nudValue_ValueChanged(object sender, EventArgs e)
        {
            if (image_array.Count > 1)
                BallDetection.Instance.Detect(this, image_array.Last(), (int)nudValue.Value);
        }
    }
}
