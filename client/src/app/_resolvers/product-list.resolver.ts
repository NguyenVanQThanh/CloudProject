import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { ProductService } from '../_services/product.service';
import { Product } from '../_models/product';
import { ProductParams } from '../_models/productParams';

export const productListedResolver: ResolveFn<Product[]> = (route, state) => {
  const productService = inject(ProductService);
  const productParams: ProductParams = {
    productName: route.queryParams['productName'] || '',
    categoryName: route.queryParams['categoryName']?.split(',') || [],
    vendorName: route.queryParams['vendorName']?.split(',') || [],
    status: route.queryParams['status'] || '',
    minPrice: route.queryParams['minPrice'] || null,
    maxPrice: route.queryParams['maxPrice'] || null,
    pageNumber: route.queryParams['pageNumber'] || 1,
    pageSize: route.queryParams['pageSize'] || 9,
  };
  return productService.getProducts(productParams);
};
