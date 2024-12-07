import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PaginatedResults } from '../_models/pagination';
import { Category, Product } from '../_models/product';
import { getPaginationResult, setPaginationHeaders } from './paginationHelper';
import { ProductParams } from '../_models/productParams';
import { map, Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  categoryList = signal<Category[]>([]);
  productCache = new Map();
  productParams = signal<ProductParams>(new ProductParams());
  paginationResults = signal<PaginatedResults<Product[]> | null> (null);
  getCategories() {
    return this.http.get<Category[]>(`${this.baseUrl}categories`);
  }
  getProducts(productParams: ProductParams){
    const response = this.productCache.get(Object.values(productParams).join('-'));
    if(response) {
      // this.paginationResults.set(response);
      return of(response);
    }
    let params = setPaginationHeaders(
      productParams.pageNumber,
      productParams.pageSize
    );
    params = params.append('productName', productParams.productName?? '');
    params = params.append('vendorName', productParams.vendorName?.join(',')?? '');
    params = params.append('categoryName', productParams.categoryName?.join(',')?? '');
    params = params.append('status', productParams.status?? '');
    params = params.append('minPrice', productParams.minPrice?? '');
    params = params.append('maxPrice', productParams.maxPrice?? '');
    return getPaginationResult<Product[]>(
      this.baseUrl + 'products',
      params,
      this.http
    ).pipe(
      map((response) => {
        this.productCache.set(Object.values(productParams).join('-'), response);
        return response;
      })
    )
  }
  getProduct(id : number){
    return this.http.get<Product>(`${this.baseUrl}products/${id}`);
  }
  addProduct(product : any){
    return this.http.post<Product>(`${this.baseUrl}products`,product,{});
  }
  addCategory(category: Category){
    return this.http.post(this.baseUrl + 'categories', category);
  }
  updateProduct(product: Product){
    return this.http.put(`${this.baseUrl}products/${product.id}`, product);
  }
  getProductParams(){
    return this.productParams;
  }
  setProductParams(productParams: ProductParams){
    this.productParams.set(productParams);
  }
  resetProductParams(){
    this.productParams.set(new ProductParams());
  }
}
