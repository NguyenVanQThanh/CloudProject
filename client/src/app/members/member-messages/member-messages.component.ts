import { AfterViewChecked, Component, inject, input, Input, OnInit, ViewChild } from '@angular/core';
import { Message } from '../../_models/Message';
import { MessagesService } from '../../_services/messages.service';
import { CommonModule } from '@angular/common';
import { TimeagoModule } from 'ngx-timeago';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  imports: [CommonModule, TimeagoModule, FormsModule],
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css'
})
export class MemberMessagesComponent implements OnInit, AfterViewChecked {
  @ViewChild('messageForm') messageForm? : NgForm
  @ViewChild('scrollMe') scrollContainer? : any;
  messageService = inject(MessagesService);
  userName = input.required<string>();
  messageContent = '';
  ngOnInit(): void {

  }
  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }
  sendMessage(){
    if (!this.userName()) return;
    this.messageService.sendMessage(this.userName(), this.messageContent).then(()=> {
      this.messageForm?.reset();
      this.scrollToBottom();
    })
  }
  private scrollToBottom(){
    if (this.scrollContainer){
      this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
    }
  }


}
