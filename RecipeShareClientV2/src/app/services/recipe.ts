import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { HttpHeaders } from '@angular/common/http';

export interface Recipe {
  id: number;
  title: string;
  ingredients: string;
  steps: string;
  cookingTime: number;
  dietaryTags: string;
}

@Injectable({
  providedIn: 'root'
})
export class RecipeService {
  private apiUrl = '/api/recipes';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Recipe[]> {
    return this.http.get<Recipe[]>(this.apiUrl);
  }

  getById(id: number): Observable<Recipe> {
    return this.http.get<Recipe>(`${this.apiUrl}/${id}`);
  }

  

create(recipe: Recipe): Observable < Recipe > {
  const token = localStorage.getItem('token');

  let headers = new HttpHeaders();
  if(token) {
    headers = headers.set('Authorization', `Bearer ${token}`);
  }

  return this.http.post<Recipe>(this.apiUrl, recipe, { headers }).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 400 && error.error) {
        return throwError(() => error.error);
      }
      return throwError(() => new Error('An unexpected error occurred'));
    })
  );
}

update(recipe: Recipe): Observable < any > {
  const token = localStorage.getItem('token');

  let headers = new HttpHeaders();
  if(token) {
    headers = headers.set('Authorization', `Bearer ${token}`);
  }

  return this.http.put(`${this.apiUrl}/${recipe.id}`, recipe, { headers });
}

delete (id: number): Observable < any > {
  const token = localStorage.getItem('token');

  let headers = new HttpHeaders();
  if(token) {
    headers = headers.set('Authorization', `Bearer ${token}`);
  }

  return this.http.delete(`${this.apiUrl}/${id}`, { headers });
}


  getByTag(tag: string): Observable<Recipe[]> {
    return this.http.get<Recipe[]>(`${this.apiUrl}?tag=${encodeURIComponent(tag)}`);
  }
}
