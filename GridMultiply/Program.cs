using System;
using MathNet.Numerics;
using MathNet.Numerics.Algorithms.LinearAlgebra.Mkl;

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
}
