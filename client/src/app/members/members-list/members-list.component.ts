import { Component, OnInit } from '@angular/core';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { CommonModule, NgFor } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { MemberCardComponent } from "../member-card/member-card.component";
import { Observable, take } from 'rxjs';
import { Pagination } from '../../_models/pagination';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { FormsModule, NgModel, ReactiveFormsModule } from '@angular/forms';
import { UserParams } from '../../_models/userParams';
import { User } from '../../_models/user';
import { AccountService } from '../../_services/account.service';

@Component({
  selector: 'app-members-list',
  standalone: true,
  imports: [CommonModule, MemberCardComponent,PaginationModule,FormsModule],
  templateUrl: './members-list.component.html',
  styleUrl: './members-list.component.css'
})
export class MembersListComponent implements OnInit {
  // memeber$: Observable<Member[]> | undefined;
  members: Member[] = [] ;
  pagination: Pagination | undefined;
  userParams: UserParams | undefined;
  genderList = [{ value: 'male', display: 'Males' }, { value: 'female', display: 'Females' }];
  constructor(private memberService: MembersService) {
    this.userParams = this.memberService.getUserParams();
   }
  ngOnInit(): void {
    // this.members$ = this.memberService.getMembers();
    this.loadMembers();
    // console.log(this.pagination);
  }
  loadMembers(){
    if (this.userParams){
      this.memberService.setUserParams(this.userParams);
      this.memberService.getMembers(this.userParams).subscribe({
        next: response => {
          console.log(response);
          if (response.result && response.pagination){
            this.members = response.result;
            this.pagination = response.pagination;
          }
          // console.log(this.pagination);
          // return this.pagination;
        }
      })
    }

  }
  resetFilters(){
      this.userParams = this.memberService.resetUserParams();
      this.loadMembers();
  }
  pageChanged(event: any){
    if (this.userParams && this.userParams?.pageNumber !== event.page){
      this.userParams.pageNumber = event.page;
      this.memberService.setUserParams(this.userParams);
      this.loadMembers();
    }
  }
}
