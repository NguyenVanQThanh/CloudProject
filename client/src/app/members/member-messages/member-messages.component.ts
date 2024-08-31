import { Component, Input, OnInit, ViewChild } from '@angular/core';
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
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm? : NgForm
  @Input() messages : Message[] = []
  @Input() username? : string;
  messageContent = '';
  constructor(private messageService : MessagesService){}
  ngOnInit(): void {

  }
  sendMessage(){
    if (!this.username) return;
    this.messageService.sendMessage(this.username, this.messageContent).subscribe({
      next: message => {
        this.messages.push(message);
        this.messageForm?.reset();
      }
    })
  }


}
