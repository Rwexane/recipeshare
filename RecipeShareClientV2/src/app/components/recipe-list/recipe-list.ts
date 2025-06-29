import { Component, OnInit } from '@angular/core';
import { RecipeService, Recipe } from '../../services/recipe';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';
import { AuthService } from '../../services/auth';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-recipe-list',
  templateUrl: './recipe-list.html',
  styleUrls: ['./recipe-list.css'],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule],
})
export class RecipeListComponent implements OnInit {
  recipes: Recipe[] = [];
  loading = false;
  error = '';

  searchControl = new FormControl('');

  constructor(private recipeService: RecipeService, private router: Router, private authService: AuthService) { }

  isSystemAdmin(): boolean {
    return this.authService.isSystemAdmin();
  }

  ngOnInit(): void {
    this.loadRecipes();

    this.searchControl.valueChanges.pipe(
      debounceTime(500),          // wait 500ms pause in events
      distinctUntilChanged(),     // only fire if value changed
      tap(() => {
        this.loading = true;
        this.error = '';
      }),
      switchMap(term => {
        const trimmed = term?.trim() ?? '';
        if (!trimmed) {
          return this.recipeService.getAll();
        }
        return this.recipeService.getByTag(trimmed);
      })
    ).subscribe({
      next: (recipes) => {
        this.recipes = recipes;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to search recipes.';
        this.loading = false;
        console.error(err);
      }
    });
  }

  loadRecipes(): void {
    this.loading = true;
    this.recipeService.getAll().subscribe({
      next: (data) => {
        this.recipes = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load recipes.';
        this.loading = false;
      },
    });
  }

  deleteRecipe(id: number): void {
    if (confirm('Are you sure you want to delete this recipe?')) {
      this.recipeService.delete(id).subscribe({
        next: () => {
          this.recipes = this.recipes.filter((r) => r.id !== id);
        },
        error: () => alert('Failed to delete the recipe.'),
      });
    }
  }

  goToDetails(id: number): void {
    this.router.navigate(['/recipes', id]);
  }

  goToEdit(id: number): void {
    this.router.navigate(['/recipes', id, 'edit']);
  }

  getBadgeClass(tag: string): string {
    const tagLower = tag.toLowerCase();
    if (tagLower.includes('vegan')) return 'badge-vegan';
    if (tagLower.includes('gluten')) return 'badge-glutenfree';
    if (tagLower.includes('dairy')) return 'badge-dairyfree';
    if (tagLower.includes('vegetarian')) return 'badge-vegetarian';
    return 'bg-secondary text-white';
  }

  getIcon(title: string): string {
    const lower = title.toLowerCase();
    if (lower.includes('cake') || lower.includes('dessert')) return '🧁';
    if (lower.includes('noodle') || lower.includes('soup')) return '🍜';
    if (lower.includes('chicken') || lower.includes('beef') || lower.includes('meat')) return '🥩';
    if (lower.includes('salad')) return '🥗';
    if (lower.includes('pizza')) return '🍕';
    if (lower.includes('fish')) return '🐟';
    return '🍽️';
  }
}
