using BenchmarkDotNet.Attributes;

namespace RecipeShare.Benchmarks
{
    [MemoryDiagnoser]
    public class RecipeApiBenchmark
    {
        private readonly HttpClient _client;

        public RecipeApiBenchmark()
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7221")
            };
        }

        [Benchmark]
        public async Task GetRecipes()
        {
            var response = await _client.GetAsync("/api/recipes");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
        }
    }
}