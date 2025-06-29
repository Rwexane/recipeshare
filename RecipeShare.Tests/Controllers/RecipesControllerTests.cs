using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using RecipeShare.Application.DTOs;
using System.Net.Http.Json;

public class RecipesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public RecipesControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsOkResult()
    {
        var response = await _client.GetAsync("/api/recipes");

        response.EnsureSuccessStatusCode(); // Throws if not 200-299
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
            DietaryTags = "Test"
        };

        // POST
        var postResponse = await _client.PostAsJsonAsync("/api/recipes", newRecipe);
        postResponse.EnsureSuccessStatusCode();

        var created = await postResponse.Content.ReadFromJsonAsync<RecipeDto>();
        created.Should().NotBeNull();
        created!.Id.Should().BeGreaterThan(0);

        // GET by ID
        var getResponse = await _client.GetAsync($"/api/recipes/{created.Id}");
        getResponse.EnsureSuccessStatusCode();

        var fetched = await getResponse.Content.ReadFromJsonAsync<RecipeDto>();
        fetched!.Title.Should().Be("Integration Test Recipe");
    }
}