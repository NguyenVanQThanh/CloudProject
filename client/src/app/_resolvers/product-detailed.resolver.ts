import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { ProductService } from '../_services/product.service';
import { Product } from '../_models/product';

export const productDetailedResolver: ResolveFn<Product> = (route, state) => {
  const productService = inject(ProductService);
  return productService.getProduct(Number (route.paramMap.get('id')!));
};
