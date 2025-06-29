import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RecipeService, Recipe } from '../../services/recipe';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-recipe-form',
  templateUrl: './recipe-form.html',
  styleUrls: ['./recipe-form.css'],
  imports: [CommonModule, ReactiveFormsModule]
})
export class RecipeFormComponent implements OnInit {
  recipeForm!: FormGroup;
  isEditMode = false;
  recipeId?: number;
  loading = false;
  error = '';
  validationErrors: { [key: string]: string[] } = {};
  public Object = Object;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private recipeService: RecipeService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.recipeForm = this.fb.group({
      title: ['', Validators.required],
      ingredients: ['', Validators.required],
      steps: ['', Validators.required],
      cookingTime: [null, [Validators.required, Validators.min(1)]],
      dietaryTags: [''],
    });

    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEditMode = true;
      this.recipeId = Number(idParam);
      this.loadRecipe(this.recipeId);
    }
  }

  loadRecipe(id: number): void {
    this.loading = true;
    this.recipeService.getById(id).subscribe({
      next: (data) => {
        this.recipeForm.patchValue(data);
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load recipe for editing.';
        this.loading = false;
      },
    });
  }

  onSubmit(): void {
    this.validationErrors = {};

    if (this.recipeForm.invalid) {
      this.recipeForm.markAllAsTouched();
      return;
    }

    const formValue: Recipe = this.recipeForm.value;

    const handleError = (error: any) => {
      if (error.status === 400 && error.error) {
        this.validationErrors = error.error;
      } else {
        alert('An unexpected error occurred.');
      }
    };

    if (this.isEditMode && this.recipeId) {
      formValue.id = this.recipeId;
      this.recipeService.update(formValue).subscribe({
        next: () => this.router.navigate(['/recipes']),
        error: handleError,
      });
    } else {
      this.recipeService.create(formValue).subscribe({
        next: () => this.router.navigate(['/recipes']),
        error: handleError,
      });
    }
  }

  onCancel(): void {
    this.router.navigate(['/recipes']);
  }

}
