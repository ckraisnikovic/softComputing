using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Cvb;
using Emgu.CV.Features2D;

namespace GolfBallTracking
{
    class BallDetection
    {
        public static String filePathOfTemplate = "..\\..\\Images\\template.png";
        //Singleton class
        private static BallDetection instance = new BallDetection();

        public static BallDetection Instance
        {
            get { return instance; }
        }
        
        public Mat TemplateImage { get; set; }

        private int templateSize;
        public int TemplateSize
        {
            get { return templateSize; }
            set 
            { 
                templateSize = value;
                Size size = new Size(templateSize, templateSize);
                loadTemplateImage();
                CvInvoke.Resize(TemplateImage, TemplateImage, size, 10, 0, Inter.Lanczos4);
              
                /*
                MCvScalar m = new MCvScalar(1);
                Point p = new Point(-1,-1);
                Mat structuringElement = 
                    CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(3,3), p);
                CvInvoke.Erode(TemplateImage, TemplateImage, structuringElement, p, 1, BorderType.Default, m);
                CvInvoke.Dilate(TemplateImage, TemplateImage, structuringElement, p, 1, BorderType.Default, m);

                //CvInvoke.Threshold(TemplateImage, TemplateImage, 100, 255, ThresholdType.Binary); */

               Console.WriteLine("Template resized to " + TemplateImage.Size);
            }
        }
        
        public int ThresholdValue { get; set; }

        private BallDetection()
        {
            loadTemplateImage();
            TemplateSize = TemplateImage.Size.Width;
        }

        private void loadTemplateImage() {
            TemplateImage = new Mat(filePathOfTemplate, Emgu.CV.CvEnum.LoadImageType.Grayscale);
        }

        public void Detect(MainForm mainForm, Mat input, int value) 
        {
            Image<Gray, Byte> inputGray = input.ToImage<Gray, Byte>();
            Image<Gray, Byte> inputGrayTh = input.ToImage<Gray, Byte>();
            mainForm.showGrayImage(inputGray);

            CvInvoke.Threshold(inputGrayTh, inputGrayTh, value, 255, ThresholdType.Binary);
         //   CvInvoke.MatchTemplate(inputGrayTh, TemplateImage, inputGrayTh, TemplateMatchingType.SqdiffNormed);
            mainForm.showGrayImageThreshold(inputGrayTh);
        }

    }
}
