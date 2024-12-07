import { CartParams } from './../_models/cartParams';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@microsoft/signalr';
import { PaginatedResults } from '../_models/pagination';
import { Cart } from '../_models/cart';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  baseUrl = environment.apiUrl;
  private httpClient = inject(HttpClient);
  cartParams = signal<CartParams>(new CartParams());
  paginationResults = signal<PaginatedResults<Cart[]> | null> (null);
  constructor() { }

}
