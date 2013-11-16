using System;
using fastJSON;
using MathNet.Numerics;
using MathNet.Numerics.Algorithms.LinearAlgebra.Mkl;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using Nancy;

namespace GridMultiply
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            // Go faster button
            Control.LinearAlgebraProvider = new MklLinearAlgebraProvider();
            var uri = new Uri("http://localhost:3579");
            var nancyHost = new Nancy.Hosting.Self.NancyHost(uri);
            nancyHost.Start();
            Console.WriteLine("Nancy is listening on {0}", uri);

            Console.Write("Press any key to continue . . .");
            Console.ReadKey(true);
            nancyHost.Stop();
        }
    }

    public class MatrixMultiplier : NancyModule
    {
        public MatrixMultiplier()
        {
            Get["/Multiplier/{Id}"] = parameters => JSON.Instance.ToJSON(DoMatrixMultiply(parameters.Id).ToArray());
        }

        private const double CORRELATION = 0.5;
        private const int MATRIX_SIZE = 50;
        private const int SERIES_SIZE = 10000;

        private static Matrix<double> DoMatrixMultiply(int iter)
        {
            Console.WriteLine("Iterator: {0}", iter);

            var choleskyInput = new double[50, 50];
            var randomSeries = new double[SERIES_SIZE, MATRIX_SIZE];
            for (var i = 0; i < MATRIX_SIZE; i++)
                for (var j = 0; j < MATRIX_SIZE; j++)
                    choleskyInput[i, j] = i == j ? 1d : CORRELATION;

            var matrix = DenseMatrix.OfArray(choleskyInput);
            var factorization = matrix.Transpose().Cholesky();

            for (var i = 0; i < MATRIX_SIZE; i++)
            {
                var randomNumbers = new Normal();
                for (var j = 0; j < SERIES_SIZE; j++)
                    randomSeries[j, i] = randomNumbers.Sample();
            }

            var randomMatrix = DenseMatrix.OfArray(randomSeries);
            var choleskiedOutput = randomMatrix * factorization.Factor;

            Console.WriteLine("Result:");
            Console.WriteLine(choleskiedOutput.ToString());
            return choleskiedOutput;
        }
    }
}
