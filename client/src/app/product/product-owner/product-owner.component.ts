import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MembersService } from '../../_services/members.service';
import { ProductService } from '../../_services/product.service';
import { Pagination } from '../../_models/pagination';
import { Category, Product } from '../../_models/product';
import { ProductParams } from '../../_models/productParams';
import { AdminService } from '../../_services/admin.service';
import { AccountService } from '../../_services/account.service';
import { ProductDetailComponent } from '../product-detail/product-detail.component';
import { ProductCardComponent } from '../product-card/product-card.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { FormsModule } from '@angular/forms';
import { ProductCreateComponent } from '../product-create/product-create.component';

@Component({
  selector: 'app-product-owner',
  standalone: true,
  imports: [CommonModule, RouterModule, ProductCardComponent, PaginationModule, FormsModule, ProductCreateComponent],
  templateUrl: './product-owner.component.html',
  styleUrl: './product-owner.component.css'
})
export class ProductOwnerComponent implements OnInit {
  private accountService = inject(AccountService);
  private productService = inject(ProductService);
  private route = inject(ActivatedRoute);
  pagination : Pagination | undefined ;
  categories : Category[] = [];
  products: Product[] = [];
  user = this.accountService.currentUser();
  productParams = new ProductParams();
  createForm = false;
  ngOnInit(): void {
    if (this.user != null){
      this.productParams.vendorName = [this.user.userName];
    }
    this.getCategories();
    this.getProducts();
    // this.route.data.subscribe(data => {
    //   this.products = data['products'].result;
    //   this.pagination = data['products'].pagination;
    // });
  }
  getProducts() {
    this.productService.getProducts(this.productParams).subscribe({
      next: (response) => {
        if (response.result && response.pagination) {
          this.products = response.result;
          this.pagination = response.pagination;
        }
      }
    });
  }
  getCategories() {
    this.productService.getCategories().subscribe({
      next: (categories) => (this.categories = categories),
    });
  }
  toggleCategory(category: Category) {
    this.resetFilter();
    this.productParams.categoryName?.push(category.name);
    this.getProducts();
  }
  resetFilter(){
    this.productParams = new ProductParams();
    if (this.user != null){
      this.productParams.vendorName?.push(this.user?.userName);
    }
    this.getProducts();
  }
  checkExistsFilter(category : Category){
    return this.productParams.categoryName?.includes(category.name);
  }
  cancelRegisterMode(event : boolean){
    this.createForm = event;
  }
  pageChanged(event: any) {
    if (
      this.productParams &&
      this.productParams.pageNumber !== event.page
    ) {
      this.productParams.pageNumber = event.page;
      this.productService.setProductParams(this.productParams);
      this.getProducts();
    }
  }
  createToggle(){
    this.createForm = !this.createForm;
  }
}

