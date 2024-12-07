import { Component, inject, OnInit, signal } from '@angular/core';
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
  // member$: Observable<Member[]> | undefined;
  members: Member[] = [] ;
  pagination: Pagination | undefined;
  memberService = inject(MembersService);
  userParams = this.memberService.getUserParams();
  genderList = [{ value: 'male', display: 'Males' }, { value: 'female', display: 'Females' }];
  ngOnInit(): void {
    this.loadMembers();
  }
  loadMembers(){
    if (this.userParams){
      this.memberService.setUserParams(this.userParams());
      this.memberService.getMembers(this.userParams()).subscribe({
        next: response => {
          if (response.result && response.pagination){
            this.members = response.result;
            this.pagination = response.pagination;
          }
        }
      })
    }

  }
  resetFilters(){
      this.memberService.resetUserParams();
      this.loadMembers();
  }
  pageChanged(event: any){
    if (this.userParams && this.userParams().pageNumber !== event.page){
      this.userParams().pageNumber = event.page;
      this.memberService.setUserParams(this.userParams());
      this.loadMembers();
    }
  }
}
