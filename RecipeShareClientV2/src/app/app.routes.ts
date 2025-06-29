import { Routes } from '@angular/router';
import { RecipeListComponent } from './components/recipe-list/recipe-list';
import { RecipeDetailsComponent } from './components/recipe-detail/recipe-detail';
import { RecipeFormComponent } from './components/recipe-form/recipe-form';
import { LoginComponent } from './components/login/login';
import { inject } from '@angular/core';
import { AuthService } from './services/auth'; // keep this

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: '', redirectTo: 'recipes', pathMatch: 'full' },
  {
    path: 'recipes',
    component: RecipeListComponent,
    canActivate: [() => inject(AuthService).canActivate()]  // ✅ fixed
  },
  {
    path: 'recipes/new',
    component: RecipeFormComponent,
    canActivate: [() => inject(AuthService).canActivate()]  // ✅ fixed
  },
  {
    path: 'recipes/:id',
    component: RecipeDetailsComponent,
    canActivate: [() => inject(AuthService).canActivate()]  // ✅ fixed
  },
  {
    path: 'recipes/:id/edit',
    component: RecipeFormComponent,
    canActivate: [() => inject(AuthService).canActivate()]  // ✅ fixed
  }
];
