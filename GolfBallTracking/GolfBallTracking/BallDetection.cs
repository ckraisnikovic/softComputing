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

        public Mat TemplateImageOriginal { get; set; }
        public Mat TemplateImage { get; set; }

        private int templateSize;
        public int TemplateSize
        {
            get { return templateSize; }
            set 
            { 
               templateSize = value;
               Size size = new Size(templateSize, templateSize);
               CvInvoke.Resize(TemplateImageOriginal, TemplateImage, size, 0, 0, Inter.Lanczos4);
               Console.WriteLine("Template resized to " + TemplateImage.Size);
            }
        }
        
        public int ThresholdValue { get; set; }

        private BallDetection()
        {
            TemplateImageOriginal = new Mat(filePathOfTemplate, Emgu.CV.CvEnum.LoadImageType.Grayscale);
            TemplateImage = new Mat();
            TemplateImageOriginal.CopyTo(TemplateImage);
            TemplateSize = TemplateImage.Size.Width;  
        }

        public void Detect(MainForm mainForm, Mat input, int thresholdValue) 
        {
            Image<Gray, Byte> inputGray = input.ToImage<Gray, Byte>();
            Image<Gray, Byte> inputGrayTh = input.ToImage<Gray, Byte>();
            Mat result = new Mat();
            mainForm.showGrayImage(inputGray);

            /*
            Mat hsv = new Mat();
            CvInvoke.CvtColor(input, hsv, ColorConversion.Bgr2Hls);
            Mat[] channels = hsv.Split();
            
            Mat H = channels[0];
            Mat thresh_green = new Mat();
            Mat thresh_green_low = new Mat(); 
            Mat thresh_green_high = new Mat();
            CvInvoke.Threshold(H, thresh_green_low, 35, 255, ThresholdType.Binary);
            CvInvoke.Threshold(H, thresh_green_high, 55, 255, ThresholdType.BinaryInv);
            CvInvoke.BitwiseAnd(thresh_green_low, thresh_green_high, thresh_green);

            CvInvoke.Threshold(inputGray, inputGrayTh, thresholdValue, 255, ThresholdType.Binary);
            
            MCvScalar msc = new MCvScalar(1);
            Point p = new Point(1, 1);
            Mat structuringElement =
                    CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(3, 3), p);
            CvInvoke.Erode(inputGrayTh, inputGrayTh, structuringElement, p, 1, BorderType.Default, msc);
            CvInvoke.Dilate(inputGrayTh, inputGrayTh, structuringElement, p, 1, BorderType.Default, msc);
            CvInvoke.Erode(inputGrayTh, inputGrayTh, structuringElement, p, 1, BorderType.Default, msc);
            CvInvoke.Dilate(inputGrayTh, inputGrayTh, structuringElement, p, 1, BorderType.Default, msc);

            CvInvoke.BitwiseNot(thresh_green, thresh_green);
            Mat cleanedImg = new Mat();
            CvInvoke.BitwiseAnd(thresh_green, inputGrayTh, cleanedImg);
            CvInvoke.MatchTemplate(cleanedImg, TemplateImage, result, TemplateMatchingType.SqdiffNormed);
            CvInvoke.Threshold(result, result, 0.8, 255, ThresholdType.BinaryInv);
            */
            CvInvoke.Threshold(inputGray, inputGrayTh, thresholdValue, 255, ThresholdType.Binary);
            CvInvoke.MatchTemplate(inputGrayTh, TemplateImage, result, TemplateMatchingType.SqdiffNormed);

            result.ConvertTo(result, DepthType.Cv8U);

            Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
            CvInvoke.FindContours(result, contours, null, RetrType.List, ChainApproxMethod.ChainApproxSimple );

            mainForm.possibleBallsInFrame.Clear();
            for (int i = 0; i < contours.Size; i++)
            {
                MCvScalar color = new MCvScalar(0, 0, 255);

               // CvInvoke.DrawContours(mainForm.image_array.Last(), contours, i, color, 3, LineType.EightConnected);
                
                Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
                //Rectangle center
                int x = rect.X + rect.Size.Width / 2;
                int y = rect.Y + rect.Size.Height / 2;
                Point possibleBall = new Point(x, y);
                mainForm.possibleBallsInFrame.Add(possibleBall);
          
              // CvInvoke.Rectangle(mainForm.image_array.Last(), rect, color, 3, LineType.EightConnected);
  
            }
             DrawIfItIsBall(mainForm);
             foreach (Point p in mainForm.trueBallPoints)
                 Draw_crosshairs(mainForm, p);
        }

        public void DrawIfItIsBall(MainForm mainform)
        {
            MCvScalar color = new MCvScalar(0, 0, 255);
            foreach (Point tempInFrame in mainform.possibleBallsInFrame)
            {
                bool foundSimilar = false, trueBall = false;
                foreach (Point temp in mainform.possibleBallPoints) 
                {
                    // Da li postoji neka takva
                    if (Math.Abs(temp.X - tempInFrame.X) < 2 &&
                        Math.Abs(temp.Y - tempInFrame.Y) < 2)
                    {
                        foundSimilar = true;
                        break;
                    }
                    // Da li postoji slicna
                    if (Math.Abs(temp.X - tempInFrame.X) < 10
                        || Math.Abs(temp.Y - tempInFrame.Y) < 10)
                    {
                        trueBall = true;
                        break;
                    }
                }
                if (foundSimilar == false && trueBall == false) 
                {
                    mainform.possibleBallPoints.Add(tempInFrame);
                }
                if (trueBall == true) {
                    bool addBall = true;
                    Point deleteBall = new Point();
                    foreach (Point temp in mainform.trueBallPoints) 
                    {
                        if (Math.Abs(temp.X - tempInFrame.X) < 2 &&
                        Math.Abs(temp.Y - tempInFrame.Y) < 2)
                        {
                            addBall = false;
                            deleteBall = temp;
                            break;
                        }
                    }
                    if (addBall)
                    {
                        mainform.trueBallPoints.Add(tempInFrame);
                        Rectangle rect = new Rectangle(tempInFrame.X, tempInFrame.Y, 5, 5);
                        CvInvoke.Rectangle(mainform.image_array.Last(), rect, color, 3, LineType.EightConnected);
                    }
                    
                }
            }
        }

        public void Draw_crosshairs(MainForm mainForm, Point location)
        {
            int d = 20;
            Point start = new Point(location.X - d / 2, location.Y);
            Point end = new Point(location.X + d / 2, location.Y);
            LineSegment2D line = new LineSegment2D(start, end);
            Image<Bgr, Byte> image = mainForm.image_array.Last().ToImage<Bgr, Byte>();
            image.Draw(line, new Bgr(Color.Blue), 1);
            /*
            d = 4;
            start = new Point(location.X, location.Y - d / 2);
            end = new Point(location.X, location.Y + d / 2);
            line = new LineSegment2D(start, end);
            image.Draw(line, new Bgr(Color.Blue), 1); */

            Mat output = new Mat(image.Mat, new Rectangle(new Point(0, 0), image.Size));
            mainForm.image_array.RemoveAt(mainForm.image_array.Count-1);
            mainForm.image_array.Add(output);   
        }
        
        


    }
}
