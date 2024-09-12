import { MemberMessagesComponent } from './../member-messages/member-messages.component';
import { bootstrapApplication } from '@angular/platform-browser';
import { GALLERY_CONFIG,GalleryConfig,GalleryItem,GalleryModule, ImageItem } from 'ng-gallery';
import { Component, importProvidersFrom, inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgbNavModule } from '@ng-bootstrap/ng-bootstrap';
import { TabDirective, TabsetComponent, TabsModule } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { MessagesService } from '../../_services/messages.service';
import { PresenceService } from '../../_services/presence.service';
import { AccountService } from '../../_services/account.service';
import { HubConnectionState } from '@microsoft/signalr';
@Component({
  selector: 'app-member-detail',
  standalone: true,
  imports: [CommonModule,NgbNavModule,
    GalleryModule,TabsModule,RouterModule, TimeagoModule
  ,MemberMessagesComponent],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css'
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', {static: true})  memberTabs? : TabsetComponent;
  presenceService = inject(PresenceService);
  private accountService = inject(AccountService);
  private router = inject(Router);
  member: Member = {} as Member ;
  images: GalleryItem[] = [];
  activeTab? : TabDirective;
  constructor(private memberService : MembersService, private route: ActivatedRoute, private messagesService : MessagesService){}
  ngOnDestroy(): void {
    this.messagesService.stopHubConnection();
  }
  ngOnInit(): void {
    this.route.data.subscribe({
      next: data => {
        this.member = data['member'];
        this.member && this.member.photos.map(p => {
          this.images.push( new ImageItem({src: p.url,
            thumb: p.url})
          )
        })
      }
    })
    this.route.paramMap.subscribe({
      next: _ => {
        this.onRouteParamsChange();
      }
    })
    this.route.queryParams.subscribe({
      next: params => {
        params['tab'] && this.selectTab(params['tab'])
      }
    })
    this.getImages()
  }
  onRouteParamsChange(){
    const user = this.accountService.currentUser();
    if (!user) return;
    if (this.messagesService.hubConnection?.state === HubConnectionState.Connected && this.activeTab?.heading === 'Messages'){
      this.messagesService.hubConnection.stop().then(() => {
        this.messagesService.createHubConnection(user, this.member.userName);
      })
    }
  }

  onTabActivated(data: TabDirective){
    this.activeTab = data;
    this.router.navigate([],{
      relativeTo: this.route,
      queryParams: {tab: this.activeTab.heading},
      queryParamsHandling: 'merge'
    })
    if (this.activeTab.heading === 'Messages' && this.member){
      // this.loadMessages();
      const user = this.accountService.currentUser();
      if (!user) return;
      this.messagesService.createHubConnection(user, this.member.userName);
      console.log("active success");
    } else {
      this.messagesService.stopHubConnection();
    }
  }
  // loadMessages(){
  //   this.messagesService.getMessageThread(this.member?.userName!).subscribe({
  //     next: messages => this.messages = messages
  //   })
  // }
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
