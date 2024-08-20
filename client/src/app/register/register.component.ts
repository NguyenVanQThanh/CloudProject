import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { TextInputComponent } from "../_forms/text-input/text-input.component";
import { Router } from '@angular/router';
import { DatePickerComponent } from "../_forms/date-picker/date-picker.component";

@Component({
  selector: 'app-register',
  standalone: true,
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
  imports: [CommonModule, FormsModule, ReactiveFormsModule, TextInputComponent, DatePickerComponent]
})
export class RegisterComponent implements OnInit {
  @Input() usersFromHomeComponent: any;
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup = new FormGroup({}) ;
  validationErrors: string[] | undefined = [];
  maxDate: Date = new Date();
  constructor(private accountService:AccountService, private toastr:ToastrService,
    private fb : FormBuilder, private router : Router
  ) { }
  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear()-18);
  }
  register(){
    const dob = this.getDateOnly(this.registerForm.controls['dateOfBirth'].value);
    const values = {...this.registerForm.value, dateOfBirth: dob};
    console.log(values);
    this.accountService.register(values).subscribe({
      next: response =>{
        console.log(response);
        this.router.navigateByUrl('/members')
      },
      error: error =>
        this.validationErrors=error
    })
  }
  matchValues(matchTo: string):ValidatorFn{
    return (control : AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : {notMatching : true}
    }
  }
  initializeForm(){
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      knownAs: ['', Validators.required],
      password: ['',[Validators.required,
        Validators.minLength(4)
      ]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]],
    });
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () => {
        this.registerForm.controls['confirmPassword'].updateValueAndValidity();
      }
    })
  }

  cancel(){
    this.cancelRegister.emit(false);  // Emitting event to HomeComponent to hide the registration form.  This is done using @Output decorator.  In HomeComponent, listen to this event and hide the registration form.  This is a one-way communication.  If you want to emit an event with a return value, you can use @Output() and EventEmitter<ReturnType>.  In this case, the return value would be the user object.  In HomeComponent, you would
    // console.log('cancelled');
  }
  private getDateOnly (dob: string | undefined){
    if (!dob) return;
    let theDob = new Date(dob);
    return new Date(theDob.setMinutes(theDob.getMinutes()-theDob.getTimezoneOffset())).toISOString().slice(0,10);
  }
}
