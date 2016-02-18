using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace GolfBallTracking
{
    class Kalman
    {
        //System matrices
        public Emgu.CV.Matrix<double> X0, P0;

        public Emgu.CV.Matrix<double> F { get; private set; }
        public Emgu.CV.Matrix<double> B { get; private set; }
        public Emgu.CV.Matrix<double> U { get; private set; }
        public Emgu.CV.Matrix<double> Q { get; private set; }
        public Emgu.CV.Matrix<double> H { get; private set; }
        public Emgu.CV.Matrix<double> R { get; private set; }

        public Emgu.CV.Matrix<double> State { get; set; }
        public Emgu.CV.Matrix<double> Covariance { get; set; }

        public Kalman(Emgu.CV.Matrix<double> f, Emgu.CV.Matrix<double> b, Emgu.CV.Matrix<double> u, Emgu.CV.Matrix<double> q, Emgu.CV.Matrix<double> h,
                            Emgu.CV.Matrix<double> r)
        {
            F = f; // transition matrix A
            B = b;
            U = u; //control signal
            Q = q; // process noise matrix
            H = h; //measurement matrix
            R = r; // measurement noise
        }

        public void Predict()
        {
            X0 = F * State; // +(B * U);
            P0 = F*Covariance*F.Transpose() + Q;
        }

        public void Correct(Emgu.CV.Matrix<double> z)
        {
            Emgu.CV.Matrix<double> s = H * P0 * H.Transpose() + R;
            Emgu.CV.Matrix<double> invS = new Matrix<double>(2,2);
            double koef = 1 / (s.Data[0, 0] * s.Data[1, 1] - s.Data[0, 1] * s.Data[1, 0]);
            invS.Data[0, 0] = koef * s.Data[1, 1];
            invS.Data[0, 1] = -koef * s.Data[0, 1];
            invS.Data[1, 0] = -koef * s.Data[1, 0];
            invS.Data[1, 1] = koef * s.Data[0, 0];

            Emgu.CV.Matrix<double> k = P0 * H.Transpose() * invS;
            State = X0 + (k*(z - (H * X0)));
            Emgu.CV.Matrix<double> I = new Emgu.CV.Matrix<double>(new double[P0.Rows, P0.Cols]);            
            I.SetIdentity();
            Covariance = (I - k*H)*P0;
        }

        public Emgu.CV.Matrix<double> GetMeasurement()
        {
            Emgu.CV.Matrix<double> measurementNoise = new Emgu.CV.Matrix<double>(2, 1);
            measurementNoise.SetRandNormal(new MCvScalar(), new MCvScalar(Math.Sqrt(measurementNoise[0, 0])));
            return H * State + measurementNoise;
        }

        public void GoToNextState()
        {
            Emgu.CV.Matrix<double> processNoise = new Emgu.CV.Matrix<double>(4, 1);
            processNoise.SetRandNormal(new MCvScalar(), new MCvScalar(processNoise[0, 0]));
            State = F * State + processNoise;
        }
    }
}
