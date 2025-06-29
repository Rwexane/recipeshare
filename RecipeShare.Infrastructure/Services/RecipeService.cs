using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using RecipeShare.Application.DTOs;
using RecipeShare.Application.Interfaces;
using RecipeShare.Domain.Interfaces;
using RecipeShare.Domain.Models;
using RecipeShare.Infrastructure.Data;

namespace RecipeShare.Infrastructure.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _repo;
        private readonly AppDbContext _context;

        public RecipeService(IRecipeRepository repo, AppDbContext context)
        {
            _repo = repo;
            _context = context;
        }

        [Benchmark]
        public async Task<List<Recipe>> GetAllAsync(string? tag = null)
        {
            IQueryable<Recipe> query = _context.Recipes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(tag))
            {
                query = query.Where(r => r.DietaryTags != null && r.DietaryTags.Contains(tag));
            }

            return await query.ToListAsync();
        }

        public async Task<RecipeDto?> GetByIdAsync(int id)
        {
            Recipe? recipe = await _repo.GetByIdAsync(id);
            return recipe == null ? null : ToDto(recipe);
        }

        public async Task<RecipeDto> CreateAsync(RecipeDto dto)
        {
            Recipe entity = ToEntity(dto);
            Recipe created = await _repo.AddAsync(entity);
            return ToDto(created);
        }

        public async Task UpdateAsync(RecipeDto dto)
        {
            Recipe entity = ToEntity(dto);
            await _repo.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id) => await _repo.DeleteAsync(id);

        private RecipeDto ToDto(Recipe r) => new()
        {
            Id = r.Id,
            Title = r.Title,
            Ingredients = r.Ingredients,
            Steps = r.Steps,
            CookingTime = r.CookingTime,
            DietaryTags = r.DietaryTags
        };

        private Recipe ToEntity(RecipeDto dto) => new()
        {
            Id = dto.Id ?? 0,
            Title = dto.Title,
            Ingredients = dto.Ingredients,
            Steps = dto.Steps,
            CookingTime = dto.CookingTime,
            DietaryTags = dto.DietaryTags
        };
    }
}