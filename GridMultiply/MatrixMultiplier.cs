using System;
using System.Collections.Generic;
using fastJSON;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
using Nancy;

namespace GridMultiply
{
    public class MatrixMultiplier : NancyModule
    {
        public MatrixMultiplier()
        {
            Get["/(About|Help)"] = parameters => JSON.Instance.ToJSON(new Dictionary<string, object>
            {
                {"Name", "GridMultiplier"}, 
                { "Author", "Justin Dearing @zippy1981"},
                { "Makes Use Of", new Dictionary<string, string>
                    {
                        { "fastJSON", "https://fastjson.codeplex.com/" },
                        { "Math.NET Numerics", "http://www.mathdotnet.com/" }
                    }
                }
            });
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