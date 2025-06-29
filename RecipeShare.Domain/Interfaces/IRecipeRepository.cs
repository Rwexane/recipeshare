using RecipeShare.Domain.Models;

namespace RecipeShare.Domain.Interfaces
{
    public interface IRecipeRepository
    {
        Task<List<Recipe>> GetAllAsync(string? tag = null);
        Task<Recipe?> GetByIdAsync(int id);
        Task<Recipe> AddAsync(Recipe recipe);
        Task UpdateAsync(Recipe recipe);
        Task DeleteAsync(int id);
    }
}