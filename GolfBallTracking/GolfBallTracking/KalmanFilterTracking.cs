using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Data;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;

namespace GolfBallTracking
{
    class KalmanFilterTracking
    {
        internal class VectorData
        {
            public Emgu.CV.Matrix<double> state;
            public Emgu.CV.Matrix<double> transitionMatrix;
            public Emgu.CV.Matrix<double> measurementMatrix;
            public Emgu.CV.Matrix<double> processNoise;
            public Emgu.CV.Matrix<double> measurementNoise;
            public Emgu.CV.Matrix<double> errorCovariancePost;
            public Emgu.CV.Matrix<double> invMeasurementNoise;

            public VectorData()
            {
                transitionMatrix = new Emgu.CV.Matrix<double>(new double[,] {
                    {1, 0, 0, 1f, 0, 0},  // x-pos, y-pos, z-pos, velocities (no accel)
					{0, 1, 0, 0, 1f, 0},
					{0, 0, 1, 0, 0, 1f},
					{0, 0, 0, 1, 0, 0},
					{0, 0, 0, 0, 1, 0},
					{0, 0, 0, 0, 0, 1},
				});
                measurementMatrix = new Emgu.CV.Matrix<double>(new double[,] {
                    { 1, 0, 0, 0, 0, 0},
					{ 0, 1, 0, 0, 0, 0},
					{ 0, 0, 1, 0, 0, 0},
				});
                measurementMatrix.SetIdentity();
                processNoise = new Emgu.CV.Matrix<double>(6, 6);
                processNoise.SetIdentity(new MCvScalar(1.0e-2));
                measurementNoise = new Emgu.CV.Matrix<double>(3, 3);
                measurementNoise.SetIdentity(new MCvScalar(1.0e-2));
                errorCovariancePost = new Emgu.CV.Matrix<double>(6, 6);
                errorCovariancePost.SetIdentity();
                invMeasurementNoise = new Emgu.CV.Matrix<double>(3, 3);
            }

            public Emgu.CV.Matrix<double> GetMeasurement()
            {
                Emgu.CV.Matrix<double> measurementNoise = new Emgu.CV.Matrix<double>(3, 1);
                measurementNoise.SetRandNormal(new MCvScalar(), new MCvScalar(Math.Sqrt(measurementNoise[0, 0])));
                return measurementMatrix * state + measurementNoise;
            }

            public void GoToNextState()
            {
                Emgu.CV.Matrix<double> processNoise = new Emgu.CV.Matrix<double>(6, 1);
                processNoise.SetRandNormal(new MCvScalar(), new MCvScalar(processNoise[0, 0]));
                state = transitionMatrix * state + processNoise;
            }
        }

        Emgu.CV.Matrix<double> stateCovariance;
        Emgu.CV.Matrix<double> state;
        KalmanFilterTracking.VectorData syntheticData;
        int L; // states
        int m; // measurements
        double c; // scaling factor
        double lambda; // scaling factor
        Emgu.CV.Matrix<double> meansWeights;
        Emgu.CV.Matrix<double> covarianceWeights;
        Emgu.CV.Matrix<double> covarianceWeightsDiagonal;

        Emgu.CV.Matrix<double> KalmanGain;
        double alpha = 1e-3;
        double ki = 0; // default 0. has not effect apparently unless the transition is non linear
        double beta = 2;

        Emgu.CV.Matrix<double> trans_sigmaPoints;
        Emgu.CV.Matrix<double> trans_stateCovariance;
        Emgu.CV.Matrix<double> trans_deviation;
        Emgu.CV.Matrix<double> trans_mean_mat;

        Emgu.CV.Matrix<double> trans_cross_covariance;

        public KalmanFilterTracking(int states, int measurements)
        {
            this.L = states;
            this.m = measurements;

            state = new Matrix<double>(this.L, 1);
            state[0, 0] = 1;
            state[1, 0] = 1;
            state[2, 0] = 1;
            state[3, 0] = 0.5;
            state[4, 0] = 0.5;
            state[5, 0] = 0.5;
            state[5, 0] = 0.1;

            stateCovariance = new Matrix<double>(L, L);
            stateCovariance.SetIdentity(new MCvScalar(1.0));

            meansWeights = new Matrix<double>(1, 2 * this.L + 1);
            covarianceWeights = new Matrix<double>(1, 2 * this.L + 1);
            covarianceWeightsDiagonal = new Matrix<double>(2 * this.L + 1, 2 * this.L + 1);

            calculateVariables();

            syntheticData = new VectorData();
        }

        public float[] update(float[] point)
        {
            //generateSigmaPoints();
            //unscentedTransformation(syntheticData.transitionMatrix, sigmaPoints, L, syntheticData.processNoise);

            var x1 = trans_mean_mat;
            var x_capital_1 = trans_sigmaPoints;
            var P1 = trans_stateCovariance;
            var x_capital_2 = trans_deviation;

           // unscentedTransformation(syntheticData.measurementMatrix, x_capital_1, m, syntheticData.measurementNoise);

            //updating
            trans_cross_covariance = x_capital_2 * covarianceWeightsDiagonal * trans_deviation.Transpose();

            // inverse of P2 (trans_covariance)
            Emgu.CV.Matrix<double> inv_trans_covariance = new Matrix<double>(trans_stateCovariance.Rows, trans_stateCovariance.Cols);
            CvInvoke.Invert(trans_stateCovariance, inv_trans_covariance, DecompMethod.Svd);
            KalmanGain = trans_cross_covariance * inv_trans_covariance;

            Emgu.CV.Matrix<double> thisMeasurement = new Matrix<double>(m, 1);
            thisMeasurement[0, 0] = point[0];
            thisMeasurement[1, 0] = point[1];
            thisMeasurement[2, 0] = point[2];

            //update state
            state = x1 + KalmanGain * (thisMeasurement - trans_mean_mat);

            //update covariance
            stateCovariance = P1 - KalmanGain * trans_cross_covariance.Transpose();

            return new float[]{(float)state[0, 0], (float)state[1, 0], (float)state[2, 0]};

        }

        private void unscentedTransformation(Emgu.CV.Matrix<double> map, Emgu.CV.Matrix<double> points, int outputs, Emgu.CV.Matrix<double> additiveCovariance)
        {
            int sigma_point_number = points.Cols; // try points.cols better
            trans_mean_mat = new Matrix<double>(outputs, 1);
            trans_sigmaPoints = new Matrix<double>(outputs, sigma_point_number);

            for (int i = 0; i < sigma_point_number; i++)
            {
                Emgu.CV.Matrix<double> transformed_point = map * points.GetCol(i);
                trans_mean_mat += meansWeights[0, i] * transformed_point;

                // store transformed point
                for (int j = 0; j < outputs; j++)
                {
                    trans_sigmaPoints[j, i] = transformed_point[j, 0];
                }
            }

            Emgu.CV.Matrix<double> intermediate_matrix_1 = new Matrix<double>(trans_mean_mat.Rows, sigma_point_number);
            for (int i = 0; i < sigma_point_number; i++)
            {
                for (int j = 0; j < trans_mean_mat.Rows; j++)
                {
                    intermediate_matrix_1[j, i] = trans_mean_mat[j, 0];
                }
            }

            trans_deviation = trans_sigmaPoints - intermediate_matrix_1; // Y1=Y-y(:,ones(1,L));

            trans_stateCovariance = trans_deviation * covarianceWeightsDiagonal * trans_deviation.Transpose() + additiveCovariance;

        }

        private void calculateVariables()
        {
            lambda = Math.Pow(alpha, 2.0) * (L + ki) - L;
            c = L + lambda;

            // means weights
            meansWeights[0, 0] = (double)(lambda / c);

            for (int i = 1; i < 2 * L + 1; i++)
            {
                meansWeights[0, i] = (double)(0.5f / c);
            }

            // cov weights
            covarianceWeights = meansWeights.Clone();
            covarianceWeights[0, 0] += (double)(1 - Math.Pow(alpha, 2.0) + beta);

            // diag of wc
            for (int i = 0; i < covarianceWeights.Cols; i++)
            {
                covarianceWeightsDiagonal[i, i] = covarianceWeights[0, i];
            }

            c = Math.Sqrt(c);
        }
      
    }
}
