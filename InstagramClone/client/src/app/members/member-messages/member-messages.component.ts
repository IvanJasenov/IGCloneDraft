import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {

  @Input() recipientId: number;
  @Input() recipientUsername: string;
  messageThread: Message[];
  messageContent: string;
  // will need the form reference in order to reset it when message send
  @ViewChild('messageForm', {static: false}) messageForm: NgForm;

  constructor(private messageService: MessageService) { }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages() {
    console.log('load message thread with userId:', this.recipientId);
    console.log('load message thread with username:', this.recipientUsername);
    this.messageService.getMessageThread(this.recipientUsername).subscribe(response => {
      this.messageThread = response;
      console.log('message thread:', this.messageThread);
    });
  }

  onSubmit() {
    console.log('send message to:', this.recipientUsername);
    console.log('content:', this.messageContent);
    this.messageService.sendMessage(this.recipientUsername, this.messageContent).subscribe((res: Message) => {
      console.log('created message:', res);
      this.loadMessages();
      this.messageForm.reset();
    });
  }

}
