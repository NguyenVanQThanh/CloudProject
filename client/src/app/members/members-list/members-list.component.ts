import { Component, OnInit } from '@angular/core';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { CommonModule, NgFor } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { MemberCardComponent } from "../member-card/member-card.component";
import { Observable } from 'rxjs';

@Component({
  selector: 'app-members-list',
  standalone: true,
  imports: [CommonModule, NgFor, MemberCardComponent],
  templateUrl: './members-list.component.html',
  styleUrl: './members-list.component.css'
})
export class MembersListComponent implements OnInit {
  members$: Observable<Member[]> | undefined ;
  constructor(private memberService: MembersService) { }
  ngOnInit(): void {
    this.members$ = this.memberService.getMembers();
  }

}
