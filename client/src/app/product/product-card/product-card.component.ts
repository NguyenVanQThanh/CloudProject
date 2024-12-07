import { CommonModule } from '@angular/common';
import { Component, inject, input, OnInit } from '@angular/core';
import { Router, RouterLink, RouterModule } from '@angular/router';
import { Product } from '../../_models/product';
import { ProductService } from '../../_services/product.service';
import { AccountService } from '../../_services/account.service';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [CommonModule, RouterModule, RouterLink],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.css'
})
export class ProductCardComponent implements OnInit {
  product = input.required<Product>();
  private accountService = inject(AccountService);
  private productService = inject(ProductService);
  private router = inject(Router);
  ngOnInit(): void {

  }
  addToCart(){

  }
  productOwner(product : Product){
    return this.product().vendor === this.accountService.currentUser()?.userName;
  }
}
