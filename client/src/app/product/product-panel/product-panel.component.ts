import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../_services/product.service';
import { Category, Product } from '../../_models/product';
import { ProductParams } from '../../_models/productParams';
import { Pagination } from '../../_models/pagination';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ProductCardComponent } from '../product-card/product-card.component';
import { User } from '../../_models/user';
import { MembersService } from '../../_services/members.service';
import { Member } from '../../_models/member';

@Component({
  selector: 'app-product-panel',
  standalone: true,
  imports: [CommonModule,PaginationModule, ProductCardComponent, FormsModule],
  templateUrl: './product-panel.component.html',
  styleUrl: './product-panel.component.css',
})
export class ProductPanelComponent implements OnInit {
  private memberService = inject(MembersService);
  private productService = inject(ProductService);
  pagination: Pagination | undefined ;
  vendors: Member[] = [];
  categories: Category[] = [];
  products: Product[] = [];
  productParams = this.productService.getProductParams();
  searchText='';
  ngOnInit(): void {
    this.productParams.set(new ProductParams());
    this.getCategories();
    this.getVendors();
    this.getProducts();
  }
  getCategories() {
    this.productService.getCategories().subscribe({
      next: (categories) => (this.categories = categories),
    });
  }
  getProducts() {
    this.productService.getProducts(this.productParams()).subscribe({
      next: (response) => {
        if (response.result && response.pagination) {
          this.products = response.result;
          this.pagination = response.pagination;
          console.log(this.pagination);
        }
      }
    });
  }
  pageChanged(event: any) {
    if (
      this.productParams() &&
      this.productParams().pageNumber !== event.page
    ) {
      this.productParams().pageNumber = event.page;
      this.productService.setProductParams(this.productParams());
      this.getProducts();
    }
  }
  toggleCategory(category: Category) {
    if (!this.productParams().categoryName) {
      this.productParams().categoryName = [category.name];
    } else {
      const index = this.productParams().categoryName?.findIndex(c => c === category.name);
      if (index === -1) {
        this.productParams().categoryName?.push(category.name);
      } else {
        this.productParams().categoryName = this.productParams().categoryName?.filter((c, i) => i !== index);
      }
    }
    this.getProducts();
  }
  toggleVendor(vendor: Member){
    if (!this.productParams().vendorName) {
      this.productParams().vendorName = [vendor.userName];
    } else {
      const index = this.productParams().vendorName?.findIndex(c => c === vendor.userName);
      if (index === -1) {
        this.productParams().vendorName?.push(vendor.userName);
      } else {
        this.productParams().vendorName = this.productParams().vendorName?.filter((c, i) => i !== index);
      }
    }
    this.getProducts();
  }
  getVendors() {
    this.memberService.getVendors().subscribe({
      next: (users) => {
        this.vendors = users;
      }
    });
  }
  existCategoryParams(category : Category){
    return this.productParams().categoryName?.findIndex(c=>c === category.name);
  }
  existVendorParams(vendor : Member){
    return this.productParams().vendorName?.findIndex(c=>c === vendor.userName);
  }
  search() {
    this.productParams().productName = this.searchText;
    this.getProducts();
  }
  resetFilters() {
    this.searchText = '';
    this.productService.resetProductParams();
    this.getProducts();
  }
}
