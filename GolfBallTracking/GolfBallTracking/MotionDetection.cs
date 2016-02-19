using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV.VideoSurveillance;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Cvb;
using Emgu.CV.Features2D;

namespace GolfBallTracking
{
    class MotionDetection
    {
        // Singleton class
        private static MotionDetection instance = new MotionDetection();
        public static MotionDetection Instance
        {
            get { return instance; }
        }

        BackgroundSubtractorMOG2 bg_model;
        Mat fg_estimate;
        int tracksCounter;

        private MotionDetection() 
        {
            fg_estimate = new Mat();
            bg_model = new BackgroundSubtractorMOG2(5, 5, true);
            tracksCounter = 0;
        }

        public void ProcessFrame(MainForm mainForm)
        {
            Mat image = mainForm.image_array.Last();
            bg_model.Apply(image, fg_estimate);
          
        }

        public void Draw_crosshairs(MainForm mainForm, Point location)
        {
            int d = 4;
            Point start = new Point(location.X - d / 2, location.Y);
            Point end = new Point(location.X + d / 2, location.Y);
            LineSegment2D line = new LineSegment2D(start, end);
            Image<Bgr, Byte> image = mainForm.image_array.Last().ToImage<Bgr, Byte>();
            image.Draw(line, new Bgr(Color.Blue), 1);

            d = 4;
            start = new Point(location.X, location.Y - d / 2);
            end = new Point(location.X, location.Y + d / 2);
            line = new LineSegment2D(start, end);
            image.Draw(line, new Bgr(Color.Blue), 1);

            Mat output = new Mat(image.Mat, new Rectangle(new Point(0, 0), image.Size));
            mainForm.image_array.RemoveAt(mainForm.image_array.Count - 1);
            mainForm.image_array.Add(output);
        }
       
    }
}
