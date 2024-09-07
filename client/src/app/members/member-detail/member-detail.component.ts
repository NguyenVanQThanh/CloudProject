import { MemberMessagesComponent } from './../member-messages/member-messages.component';
import { bootstrapApplication } from '@angular/platform-browser';
import { GALLERY_CONFIG,GalleryConfig,GalleryItem,GalleryModule, ImageItem } from 'ng-gallery';
import { Component, importProvidersFrom, OnInit, ViewChild } from '@angular/core';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgbNavModule } from '@ng-bootstrap/ng-bootstrap';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { MessagesService } from '../../_services/messages.service';
import { Message } from '../../_models/Message';
@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [CommonModule,NgbNavModule,
    GalleryModule,TabsModule,RouterModule, TimeagoModule
  ,MemberMessagesComponent],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css'
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs', {static: true})  memberTabs? : TabsetComponent;
  member: Member = {} as Member ;
  images: GalleryItem[] = [];
  activeTab? : TabDirective;
  messages : Message[] = [];
  constructor(private memberService : MembersService, private route: ActivatedRoute, private messagesService : MessagesService){}
  ngOnInit(): void {
    this.route.data.subscribe({
      next: data => this.member = data['member']
    })
    this.route.queryParams.subscribe({
      next: params => {
        params['tab'] && this.selectTab(params['tab'])
      }
    })
    this.getImages()
  }

  onTabActivated(data: TabDirective){
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages'){
      this.loadMessages();
    }
  }
  loadMessages(){
    this.messagesService.getMessageThread(this.member?.userName!).subscribe({
      next: messages => this.messages = messages
    })
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
  selectTab(heading: string){
    if (this.memberTabs){
      const messageTab = this.memberTabs.tabs.find(x => x.heading === heading);
      if (messageTab) messageTab.active = true;
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
    },
    importProvidersFrom(TimeagoModule.forRoot()),
  ]
})
