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
        public List<Mat> image_array = new List<Mat>();
        public List<Point> possibleBallPoints = new List<Point>();
        public List<Point> possibleBallsInFrame = new List<Point>();
        public List<Point> trueBallPoints = new List<Point>();
        public List<Point> kalmanPoints = new List<Point>();
        private Kalman kalman = null;

        public MainForm()
        {
            InitializeComponent();
            FPS = 30;
            tracking = false;
            nudTemplateSize.Value = nudTemplateSize.Maximum;
            showTemplateInPictureBox();
            KalmanFunctionInit();
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
                    if (image_array.Count >= 6)
                    {
                        image_array.RemoveAt(0);
                        possibleBallPoints.Clear();
                    }
                    image_array.Add(frame);
                    BallDetection.Instance.Detect(this, image_array.Last(), (int)nudValue.Value);
                    camImageBox.Image = image_array.Last();

                    MCvScalar color = new MCvScalar(255, 0, 0);
                    foreach (Point p in trueBallPoints) 
                    {
                        kalman.State = new Matrix<double>(new Double[4, 1] { { p.X }, { p.Y }, { 0.5 }, { 0.5 } });
                        kalman.Predict();
                        kalman.Correct(kalman.GetMeasurement());
                        kalman.GoToNextState();
                        Point estimated = new Point((int)Math.Ceiling(kalman.State.Data[0,0]), 
                            (int)Math.Ceiling(kalman.State.Data[1,0]));
                        Rectangle rect = new Rectangle(estimated.X, estimated.Y, 1, 1);
                        CvInvoke.Rectangle(image_array.Last(), rect, color, 2, LineType.EightConnected);
                        kalmanPoints.Add(estimated);

                        kalman.State = new Matrix<double>(new Double[4, 1] { { estimated.X }, { estimated.Y }, { 0.5 }, { 0.5 } });
                        kalman.Predict();
                        kalman.Correct(kalman.GetMeasurement());
                        kalman.GoToNextState();
                        estimated = new Point((int)Math.Ceiling(kalman.State.Data[0, 0]),
                            (int)Math.Ceiling(kalman.State.Data[1, 0]));
                        rect = new Rectangle(estimated.X, estimated.Y, 1, 1);
                        CvInvoke.Rectangle(image_array.Last(), rect, color, 1, LineType.EightConnected);
                        kalmanPoints.Add(estimated);
                    }
         
                    Point[] points = new Point[2];
                    Point prevPoint = new Point(-1, -1);
                    color = new MCvScalar(0, 0, 0);
                    foreach (Point p in trueBallPoints) 
                    {
                        if (prevPoint.X < 0)
                        {
                            prevPoint = p;
                            continue;
                        }

                        if ( Math.Abs(p.X - prevPoint.X) < 20  &&
                                Math.Abs(p.Y - prevPoint.Y) < 16)
                        {
                            points[0] = prevPoint;
                            points[1] = p;
                            CvInvoke.Polylines(image_array.Last(), points, false, color, 1, LineType.EightConnected);
                        }
                       prevPoint = p;   
                    }
                    
                    
                    
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
            nudTemplateSize.Enabled = true;
            nudValue.Enabled = true;
            btnClearBgr.Enabled = true;
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
            possibleBallPoints.Clear();
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
            trueBallPoints.Clear();
            possibleBallPoints.Clear();
            possibleBallsInFrame.Clear();
            kalmanPoints.Clear();
        }

        public void showGrayImage(Image<Gray, Byte> image) 
        {
            pbBall.Image = image.Bitmap;
        }


        private void nudValue_ValueChanged(object sender, EventArgs e)
        {
            if (image_array.Count > 0)
                BallDetection.Instance.Detect(this, image_array.Last(), (int)nudValue.Value);
        }


        public void KalmanFunctionInit() 
        {
            Emgu.CV.Matrix<double> transitionMatrix;
            Emgu.CV.Matrix<double> measurementMatrix;
            Emgu.CV.Matrix<double> processNoise;
            Emgu.CV.Matrix<double> measurementNoise;
            Emgu.CV.Matrix<double> b;
            Emgu.CV.Matrix<double> u;

            transitionMatrix = new Emgu.CV.Matrix<double>(new double[,] {
                    {1, 0, 1, 0},
                    {0, 1, 0, 1},
                    {0, 0, 1, 0},
                    {0, 0, 0, 1}
				});
            measurementMatrix = new Emgu.CV.Matrix<double>(new double[,] {
                    { 1, 0, 0, 0 },
                    { 0, 1, 0, 0 }
				});
            b = new Emgu.CV.Matrix<double>(new Double[] { 0 });
            u = new Emgu.CV.Matrix<double>(new Double[] { 1 });
            measurementMatrix.SetIdentity();
            processNoise = new Emgu.CV.Matrix<double>(4, 4);
            processNoise.SetIdentity(new MCvScalar(1.0e-2));
            measurementNoise = new Emgu.CV.Matrix<double>(2, 2);
            measurementNoise.SetIdentity(new MCvScalar(1.0e-2));

            kalman = new Kalman(transitionMatrix, b, u, processNoise, measurementMatrix, measurementNoise);
            Console.WriteLine("Success.");
           // kalman.State = new Matrix<double>(new Double[4, 1] { { 1 }, { 2 }, { 0 }, { 0 } });
            kalman.Covariance = new Matrix<double>(new Double[4, 4]);
            kalman.Covariance.SetIdentity();

            /*
            kalman.Predict();
            kalman.Correct(kalman.GetMeasurement());
            kalmanPoints.Add(new Point((int)kalman.State.Data[0,0], (int)kalman.State.Data[1,0])); */
        }
    }
}
