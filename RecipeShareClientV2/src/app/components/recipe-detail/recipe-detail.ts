import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RecipeService, Recipe } from '../../services/recipe';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth';


@Component({
  selector: 'app-recipe-details',
  templateUrl: './recipe-detail.html',
  styleUrls: ['./recipe-detail.css'],
  imports: [CommonModule, ReactiveFormsModule]
})
export class RecipeDetailsComponent implements OnInit {
  recipe?: Recipe;
  loading = false;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private recipeService: RecipeService,
    private router: Router,
    private authService: AuthService
  ) { }

  isSystemAdmin(): boolean {
    return this.authService.isSystemAdmin();
  }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.loadRecipe(id);
    } else {
      this.error = 'Invalid recipe ID';
    }
  }

  loadRecipe(id: number): void {
    this.loading = true;
    this.recipeService.getById(id).subscribe({
      next: (data) => {
        this.recipe = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Recipe not found.';
        this.loading = false;
      },
    });
  }

  goBack(): void {
    this.router.navigate(['/recipes']);
  }

  goToEdit(): void {
    if (this.recipe) {
      this.router.navigate(['/recipes', this.recipe.id, 'edit']);
    }
  }

  getIconForTitle(title: string): string {
    const t = title.toLowerCase();
    if (t.includes('cake') || t.includes('dessert')) return '🧁';
    if (t.includes('soup') || t.includes('noodle')) return '🍜';
    if (t.includes('chicken')) return '🍗';
    if (t.includes('beef')) return '🥩';
    return '🍽️';
  }

}
