import { Component, computed, inject, input, Input, OnInit } from '@angular/core';
import { Member } from '../../_models/member';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterModule } from '@angular/router';
import { MembersService } from '../../_services/members.service';
import { ToastrService } from 'ngx-toastr';
import { PresenceService } from '../../_services/presence.service';
import { LikesService } from '../../_services/likes.service';

@Component({
  selector: 'app-member-card',
  standalone: true,
  imports: [CommonModule,RouterModule, RouterLink],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css'
})
export class MemberCardComponent implements OnInit {
  member = input.required<Member>();
  private likesService = inject(LikesService);
  private presenceService = inject(PresenceService);
  hasLiked = computed(()=> this.likesService.likeIds().includes(this.member().id));
  isOnline = computed(()=> this.presenceService.onlineUsers().includes(this.member().userName));
  private router = inject(Router);
  constructor(private memberService: MembersService, private toastr:ToastrService) {

   }
  ngOnInit(): void {
    console.log(this.hasLiked)
  }
  toggleLike(){
    this.likesService.toggleLike(this.member().id).subscribe({
      next: () => {
        if (this.hasLiked()){
          this.likesService.likeIds.update(ids => ids.filter(x=>x !== this.member().id))
          this.toastr.error('You have unliked '+this.member().knownAs)
        } else {
          this.likesService.likeIds.update(ids => [...ids, this.member().id])
          this.toastr.success('You have liked '+this.member().knownAs)
        }
      }
    })
  }

}
