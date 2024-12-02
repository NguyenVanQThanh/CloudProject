import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PaginatedResults } from '../_models/pagination';
import { Category, Product } from '../_models/product';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  categoryList = signal<Category[]>([]);
  paginationResults = signal<PaginatedResults<Product[]> | null> (null);
  getCategories() {
    return this.http.get<Category[]>(`${this.baseUrl}categories`).subscribe({
      next: categories => this.categoryList.set(categories)
    })
  }
}
