using System.ComponentModel.DataAnnotations;

namespace RecipeShare.Application.DTOs
{
    public class RecipeDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingredients are required.")]
        public string Ingredients { get; set; } = string.Empty;

        [Required(ErrorMessage = "Steps are required.")]
        public string Steps { get; set; } = string.Empty;

        [Range(1, 1440, ErrorMessage = "Cooking time must be between 1 and 1440 minutes.")]
        public int CookingTime { get; set; }

        public string? DietaryTags { get; set; }
    }
}