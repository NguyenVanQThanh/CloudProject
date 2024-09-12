import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { MembersService } from '../_services/members.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MemberCardComponent } from '../members/member-card/member-card.component';
import { Pagination } from '../_models/pagination';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { LikesService } from '../_services/likes.service';

@Component({
  selector: 'app-lists',
  standalone: true,
  imports: [CommonModule,FormsModule,MemberCardComponent,PaginationModule,ButtonsModule],
  templateUrl: './lists.component.html',
  styleUrl: './lists.component.css'
})
export class ListsComponent implements OnInit, OnDestroy{
  members : Member[] | undefined;
  predicate = 'liked';
  pageNumber = 1;
  pageSize = 5;
  pagination: Pagination | undefined;
  private likeService = inject(LikesService);
  constructor(private memberService: MembersService) { }
  ngOnDestroy(): void {
    this.likeService.paginatedResults.set(null);
  }
  ngOnInit(): void {
    this.loadLikes();
  }
  getTitle(){
    switch (this.predicate){
      case 'liked': return 'Members you like';
      case 'likedBy': return 'Members who like you';
      default: return 'Mutual'
    }
  }
  loadLikes(){
    this.likeService.getLikes(this.predicate, this.pageNumber, this.pageSize);
  }
  pageChanged(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadLikes();
    }
  }
}
