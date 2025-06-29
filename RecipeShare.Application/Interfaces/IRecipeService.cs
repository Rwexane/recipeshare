using RecipeShare.Application.DTOs;
using RecipeShare.Domain.Models;

namespace RecipeShare.Application.Interfaces
{
    public interface IRecipeService
    {
        Task<List<Recipe>> GetAllAsync(string? tag = null);
        Task<RecipeDto?> GetByIdAsync(int id);
        Task<RecipeDto> CreateAsync(RecipeDto dto);
        Task UpdateAsync(RecipeDto dto);
        Task DeleteAsync(int id);
    }
}