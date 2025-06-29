using Microsoft.EntityFrameworkCore;
using RecipeShare.Domain.Models;
using RecipeShare.Infrastructure.Models;

namespace RecipeShare.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Recipe> Recipes => Set<Recipe>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure composite key for UserRole
            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });

            // Configure relationships
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // Seed Recipes
            modelBuilder.Entity<Recipe>().HasData(
                new Recipe { Id = 1, Title = "Chicken Curry", Ingredients = "Chicken, Curry", Steps = "Cook", CookingTime = 45, DietaryTags = "Gluten-Free" },
                new Recipe { Id = 2, Title = "Salad", Ingredients = "Lettuce, Tomato", Steps = "Toss", CookingTime = 5, DietaryTags = "Vegan" },
                new Recipe { Id = 3, Title = "Beef Stew", Ingredients = "Beef, Potatoes", Steps = "Boil", CookingTime = 60, DietaryTags = "High-Protein" }
            );

            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "SystemAdmin" },
                new Role { Id = 2, Name = "StandardUser" }
            );

            // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", PasswordHash = "$2y$10$8SAsAgOuPG294fzcbEFD4eIvkXrlPyg8H6gB6bLuwcZ.AYoNbxvye" },
                new User { Id = 2, Username = "john", PasswordHash = "$2y$10$8SAsAgOuPG294fzcbEFD4eIvkXrlPyg8H6gB6bLuwcZ.AYoNbxvye" }
            );

            // Seed UserRoles
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { UserId = 1, RoleId = 1 }, // admin -> SystemAdmin
                new UserRole { UserId = 2, RoleId = 2 }  // john -> StandardUser
            );
        }
    }
}