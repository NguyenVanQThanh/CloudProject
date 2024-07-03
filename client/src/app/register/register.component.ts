import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  standalone: true,
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
  imports: [CommonModule,FormsModule]
})
export class RegisterComponent implements OnInit {
  @Input() usersFromHomeComponent: any;
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  constructor(private accountService:AccountService) { }
  ngOnInit(): void {
  }
  register(){
    this.accountService.register(this.model).subscribe({
      next: response =>{
        console.log(response);
        this.cancel();
      },
      error: error => console.log(error)
    })
  }

  cancel(){
    this.cancelRegister.emit(false);  // Emitting event to HomeComponent to hide the registration form.  This is done using @Output decorator.  In HomeComponent, listen to this event and hide the registration form.  This is a one-way communication.  If you want to emit an event with a return value, you can use @Output() and EventEmitter<ReturnType>.  In this case, the return value would be the user object.  In HomeComponent, you would
    console.log('cancelled');
  }

}
