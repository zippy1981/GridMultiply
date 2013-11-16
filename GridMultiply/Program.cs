using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.Algorithms.LinearAlgebra.Mkl;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Double;

namespace GridMultiply
{
    internal static class Program
    {
        private const double CORRELATION = 0.5;
        private const int MATRIX_SIZE = 50;
        private const int SERIES_SIZE = 10000;
        private const int totalThreads = 100000;
        private const int maxConcurrentThreads = 500;
        private static readonly ParallelOptions parallelOptions;

        static Program()
        {
            parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxConcurrentThreads
            };
            int workerThreads, ioThreads;
            ThreadPool.GetMaxThreads(out workerThreads, out ioThreads);
            ThreadPool.SetMaxThreads(maxConcurrentThreads, ioThreads);
        }

        static void Main(string[] args)
        {
            // Go faster button
            Control.LinearAlgebraProvider = new MklLinearAlgebraProvider();

            Parallel.For(0, 20000, parallelOptions, DoMatrixMultiply);

            Console.Write("Press any key to continue . . .");
            Console.ReadKey(true);
        }

        private static void DoMatrixMultiply(int iter)
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
            var choleskiedOutput = randomMatrix*matrix;

            Console.WriteLine("Result:");
            Console.WriteLine(choleskiedOutput.ToString());
        }
    }
}
