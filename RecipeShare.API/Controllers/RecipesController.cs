using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeShare.Application.DTOs;
using RecipeShare.Application.Interfaces;
using RecipeShare.Domain.Models;
using System.Net;

namespace RecipeShare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<RecipeDto>>> Get([FromQuery] string? tag)
        {
            try
            {
                List<Recipe> recipes = await _recipeService.GetAllAsync(tag);
                return Ok(recipes);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Failed to fetch recipes.", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDto>> GetById(int id)
        {
            try
            {
                RecipeDto? recipe = await _recipeService.GetByIdAsync(id);
                if (recipe == null)
                    return NotFound(new { message = $"Recipe with ID {id} not found." });

                return Ok(recipe);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Error retrieving recipe.", details = ex.Message });
            }
        }

        [Authorize(Roles = "SystemAdmin")]
        [HttpPost]
        public async Task<ActionResult<RecipeDto>> Create([FromBody] RecipeDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                RecipeDto created = await _recipeService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Failed to create recipe.", details = ex.Message });
            }
        }

        [Authorize(Roles = "SystemAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RecipeDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            if (id != dto.Id)
                return BadRequest(new { message = "Recipe ID mismatch between URL and payload." });

            try
            {
                await _recipeService.UpdateAsync(dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Failed to update recipe.", details = ex.Message });
            }
        }

        [Authorize(Roles = "SystemAdmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _recipeService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "Failed to delete recipe.", details = ex.Message });
            }
        }
    }
}