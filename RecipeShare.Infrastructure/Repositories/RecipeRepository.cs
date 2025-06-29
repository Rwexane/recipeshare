using Microsoft.EntityFrameworkCore;
using RecipeShare.Domain.Interfaces;
using RecipeShare.Domain.Models;
using RecipeShare.Infrastructure.Data;

namespace RecipeShare.Infrastructure.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly AppDbContext _context;

        public RecipeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Recipe>> GetAllAsync(string? tag = null)
        {
            var query = _context.Recipes.AsQueryable();

            if (!string.IsNullOrEmpty(tag))
                query = query.Where(r => r.DietaryTags.Contains(tag));

            return await query.ToListAsync();
        }

        public async Task<Recipe?> GetByIdAsync(int id) => await _context.Recipes.FindAsync(id);

        public async Task<Recipe> AddAsync(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return recipe;
        }

        public async Task UpdateAsync(Recipe recipe)
        {
            _context.Entry(recipe).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
            }
        }
    }
}