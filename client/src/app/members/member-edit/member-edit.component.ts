import { Component, HostListener, inject, OnInit, ViewChild } from '@angular/core';
import { Member } from '../../_models/member';
import { User } from '../../_models/user';
import { HttpClient } from '@angular/common/http';
import { MembersService } from '../../_services/members.service';
import { AccountService } from '../../_services/account.service';
import { take } from 'rxjs';
import { CommonModule } from '@angular/common';
import { NgbNavModule } from '@ng-bootstrap/ng-bootstrap';
import { GALLERY_CONFIG, GalleryConfig, GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { bootstrapApplication } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { FormsModule, NgForm, ReactiveFormsModule } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { PhotoEditorComponent } from "../photo-editor/photo-editor.component";
import { TimeagoModule } from 'ngx-timeago';

@Component({
  selector: 'app-member-edit',
  standalone: true,
  imports: [CommonModule, NgbNavModule, GalleryModule, TabsModule, FormsModule, PhotoEditorComponent,
    ReactiveFormsModule, TimeagoModule
  ],
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css'
})
export class MemberEditComponent implements OnInit{
  @ViewChild('editForm') editForm: NgForm | undefined;
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any){
    if (this.editForm?.dirty){
      $event.returnValue = true;
    }
  }
  member: Member | undefined;
  images: GalleryItem[] = [];
  accountService = inject(AccountService);
  user = this.accountService.currentUser();
  constructor(private memberService:MembersService,
     private route: ActivatedRoute, private toastr : ToastrService){
  }
  ngOnInit(): void {
    this.loadMember();
  }
  loadMember(){
    if (!this.user) return;
    this.memberService.getMember(this.user.userName).subscribe({
      next: member => {
        console.log(member);
        this.member = member;
      }
    })
  }
  // loadMember(){
  //   const username = this.route.snapshot.paramMap.get('username');
  //   if (!username) return;
  //   this.memberService.getMember(username).subscribe({
  //     next: member => {
  //       this.member = member;
  //       this.getImages();
  //       // console.log(this.member);
  //     }
  //   })
  // }
  getImages(){
    if (!this.member) return;
    for (const photo of this.member.photos){
      this.images.push(new ImageItem({src: photo.url, thumb: photo.url}))
    }
  }
  updateMember(){
    this.memberService.updateMember(this.editForm?.value).subscribe({
      next: _ => {
        this.toastr.success("Profile updated successfully");
        this.editForm?.reset(this.member);
      }
    })
  }

}
bootstrapApplication(MemberEditComponent, {
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
