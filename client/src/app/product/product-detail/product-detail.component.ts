import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ProductService } from '../../_services/product.service';
import { Product } from '../../_models/product';
import { GalleryModule } from 'ng-gallery';
import { AccountService } from '../../_services/account.service';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, GalleryModule],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.css'
})
export class ProductDetailComponent implements OnInit{
  private accountService = inject(AccountService);
  private productService = inject(ProductService);
  private route = inject(ActivatedRoute);
  product: Product = {} as Product;
  ngOnInit(): void {
    this.route.data.subscribe({
      next: data =>{
        this.product = data['product'];
        console.log(this.product);
      }
    })
  }
  addToCart(product: Product){

  }
  edit(){

  }
  productOwner(product : Product){
    return this.product.vendor === this.accountService.currentUser()?.userName;
  }


}
