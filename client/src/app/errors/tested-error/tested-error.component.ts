import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-tested-error',
  standalone: true,
  imports: [],
  templateUrl: './tested-error.component.html',
  styleUrl: './tested-error.component.css'
})
export class TestedErrorComponent implements OnInit {
  baseUrl = 'https://localhost:5024/api/';
  constructor(private http : HttpClient){}
  ngOnInit(): void {

  }
  get404Error(){
    this.http.get(this.baseUrl + 'buggy/not-found').subscribe({
      next: response => console.log(response),
      error: error => console.log(error)
    })
  }
  get400Error(){
    this.http.get(this.baseUrl + 'buggy/bad-request').subscribe({
      next: response => console.log(response),
      error: error => console.log(error)
    })
  }
  get401Error(){
    this.http.get(this.baseUrl + 'buggy/auth').subscribe({
      next: response => console.log(response),
      error: error => console.log(error)
    })
  }
  get400ValidationError(){
    this.http.get(this.baseUrl + 'account/register').subscribe({
      next: response => console.log(response),
      error: error => console.log(error)
    })
  }
  get500Error(){
    this.http.get(this.baseUrl + 'buggy/server-error').subscribe({
      next: response => console.log(response),
      error: error => console.log(error)
    })
  }


}
