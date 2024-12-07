import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProductService } from '../../_services/product.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { Category } from '../../_models/product';

@Component({
  selector: 'app-product-create',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './product-create.component.html',
  styleUrl: './product-create.component.css'
})
export class ProductCreateComponent implements OnInit{
  @Output() cancelRegister = new EventEmitter();
  productForm: FormGroup = new FormGroup({});
  private fb = inject(FormBuilder);
  private productService = inject(ProductService);
  private toastr = inject(ToastrService);
  private router = inject(Router);
  categories : Category[] = [];
  validationErrors : string[] = [];
  ngOnInit(): void {
    this.loadCategory();
    this.initialize();
  }
  loadCategory(){
    this.productService.getCategories().subscribe({
      next: categories => {
        this.categories = categories;
      }
    })
  }
  initialize(){
    this.productForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      description: ['', [Validators.required, Validators.minLength(10)]],
      price: ['', [Validators.required, Validators.min(1)]],
      categoryId: ['', Validators.required],
      image: [null, Validators.required],
      quantity: ['', [Validators.required, Validators.min(1)]],
    })
  }
  createProduct(){
    if (this.productForm.valid){
      const formData = new FormData();

      formData.append('name', this.productForm.get('name')?.value);
      formData.append('price', this.productForm.get('price')?.value);
      formData.append('description', this.productForm.get('name')?.value);
      formData.append('quantity', this.productForm.get('quantity')?.value);
      formData.append('categoryId', this.productForm.get('categoryId')?.value);

      const imageFile = this.productForm.get('image')?.value;
      if (imageFile){
        formData.append('image', imageFile, imageFile.name);
      }
      this.productService.addProduct(formData).subscribe({
        next: _ => {
          this.toastr.success('Product created successfully');
          this.cancelRegister.emit();
          this.router.navigateByUrl("/");
        }, error: error => {
          this.validationErrors = error;
        }
      })
    }
  }
  onFileChange(event: any){
    const file = event.target.files[0];
    this.productForm.patchValue({image: file});
    this.productForm.updateValueAndValidity();
  }
  cancel(){
    this.cancelRegister.emit(false);
  }
}
