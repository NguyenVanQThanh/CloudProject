import { bootstrapApplication } from '@angular/platform-browser';
import { GALLERY_CONFIG,GalleryConfig,GalleryItem,GalleryModule, ImageItem } from 'ng-gallery';
import { Component, OnInit } from '@angular/core';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgbNavModule } from '@ng-bootstrap/ng-bootstrap';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [CommonModule,NgbNavModule,GalleryModule,TabsModule,RouterModule, TimeagoModule],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css'
})
export class MemberDetailComponent implements OnInit {
  active = 1;
  member: Member | undefined;
  images: GalleryItem[] = [];
  constructor(private memberService : MembersService, private route: ActivatedRoute){}
  ngOnInit(): void {
    this.loadMember();
  }

  loadMember(){
    const username = this.route.snapshot.paramMap.get('username');
    if (!username) return;
    this.memberService.getMember(username).subscribe({
      next: member => {
        this.member = member;
        this.getImages();
        // console.log(this.member);
      }
    })
  }
  getImages(){
    if (!this.member) return;
    for (const photo of this.member.photos){
      this.images.push(new ImageItem({src: photo.url, thumb: photo.url}))
    }
  }

}
bootstrapApplication(MemberDetailComponent, {
  providers: [
    {
      provide: GALLERY_CONFIG,
      useValue: {
        autoHeight: true,
        imageSize: 'cover'
      } as GalleryConfig
    }
  ]
})
