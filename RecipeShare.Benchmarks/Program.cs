using BenchmarkDotNet.Running;

namespace RecipeShare.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<RecipeApiBenchmark>();

            // Wait for user to press Enter before closing
            Console.WriteLine("\nBenchmark finished. Press Enter to exit.");
            Console.ReadLine();
        }
    }
}