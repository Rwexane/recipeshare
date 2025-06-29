using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecipeShare.API;
using RecipeShare.Application.DTOs;
using RecipeShare.Infrastructure.Data;
using System.Net.Http.Json;

public class RecipesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public RecipesControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        var configuredFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("IntegrationTest");

            builder.ConfigureServices(services =>
            {
                // Remove existing AppDbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Use a fixed in-memory database name so all test requests share the same DB
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // IMPORTANT: Do NOT build the service provider manually here
                // The test host will build it for you automatically.
            });
        });

        _client = configuredFactory.CreateClient();

        // Set fake authentication header for [Authorize]
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Test");
    }

    [Fact]
    public async Task Get_ReturnsOkResult()
    {
        var response = await _client.GetAsync("/api/recipes");
        response.EnsureSuccessStatusCode();
        var recipes = await response.Content.ReadFromJsonAsync<List<RecipeDto>>();
        recipes.Should().NotBeNull();
    }

    [Fact]
    public async Task Post_Then_GetById_Works()
    {
        var newRecipe = new RecipeDto
        {
            Title = "Integration Test Recipe",
            CookingTime = 45,
            DietaryTags = "Test",
            Ingredients = "Flour, Sugar, Eggs",
            Steps = "Mix all ingredients and bake for 30 minutes"
        };

        var postResponse = await _client.PostAsJsonAsync("/api/recipes", newRecipe);
        postResponse.EnsureSuccessStatusCode();

        var created = await postResponse.Content.ReadFromJsonAsync<RecipeDto>();
        created.Should().NotBeNull();
        created!.Id.Should().BeGreaterThan(0);

        var getResponse = await _client.GetAsync($"/api/recipes/{created.Id}");
        getResponse.EnsureSuccessStatusCode();

        var fetched = await getResponse.Content.ReadFromJsonAsync<RecipeDto>();
        fetched!.Title.Should().Be("Integration Test Recipe");
    }
}