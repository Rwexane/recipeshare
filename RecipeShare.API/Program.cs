using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecipeShare.API.Helpers;
using RecipeShare.Application.Interfaces;
using RecipeShare.Domain.Interfaces;
using RecipeShare.Infrastructure.Data;
using RecipeShare.Infrastructure.Repositories;
using RecipeShare.Infrastructure.Services;
using System.Text;

namespace RecipeShare.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var isIntegrationTest = builder.Environment.EnvironmentName == "IntegrationTest";

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // ? Conditionally register DB provider
            if (!isIntegrationTest)
            {
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(
                        builder.Configuration.GetConnectionString("DefaultConnection"),
                        sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null);
                            sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name);
                        }));
            }

            // Dependency Injection
            builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
            builder.Services.AddScoped<IRecipeService, RecipeService>();
            builder.Services.AddScoped<JwtHelper>();

            // ? Authentication
            if (isIntegrationTest)
            {
                // Fake test authentication
                builder.Services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

                builder.Services.AddAuthorization();
            }
            else
            {
                var secretKey = builder.Configuration["Jwt:SecretKey"]
                    ?? throw new InvalidOperationException("JWT SecretKey is not configured.");
                var key = Encoding.ASCII.GetBytes(secretKey);

                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidIssuer = builder.Configuration["Jwt:Issuer"],
                            ValidAudience = builder.Configuration["Jwt:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                        };
                    });

                builder.Services.AddAuthorization();
            }

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.MapControllers();
            app.MapFallbackToFile("index.html");

            // ? Apply migrations only in real env
            if (!isIntegrationTest)
            {
                using var scope = app.Services.CreateScope();
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<AppDbContext>();
                    dbContext.Database.Migrate();
                    Console.WriteLine("Database migrations applied successfully.");
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while applying database migrations.");
                }
            }

            app.Run();
        }
    }
}