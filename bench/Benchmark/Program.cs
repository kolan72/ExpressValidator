using BenchmarkDotNet.Running;
using System;

namespace Benchmark
{
	static class Program
	{
		static void Main(string[] args)
		{
			BenchmarkRunner.Run<FuncFromPropertyInfoBenchmark>();
			Console.ReadKey();
		}
	}
}
