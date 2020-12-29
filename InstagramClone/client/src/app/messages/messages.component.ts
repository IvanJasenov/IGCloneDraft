import { ThrowStmt } from '@angular/compiler';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Message } from '../_models/message';
import { IPagination, PaginatedResult } from '../_models/pagination';
import { MessageService } from '../_services/message.service';
import { ViewChild } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[];
  pageNumber = 1;
  pageSize = 3;
  container = 'Inbox'; // start from the Inbox
  pagination: IPagination;
  loadingProgress = false;
  urlLink: string;

  @ViewChild('childModal', { static: false }) childModal: ModalDirective;
  confirmDelete: boolean;
  messageTodeletId: number;

  constructor(private messageService: MessageService, private router: Router) { }

  ngOnInit() {
    this.confirmDelete = false;
    this.loadMessages();
  }

  showChildModal(messageId: number) {
    this.childModal.show();
    this.confirmDelete = false;
    this.messageTodeletId = messageId;
  }

  hideChildModal(): void {
    this.childModal.hide();
    this.confirmDelete = true;
  }

  loadMessages() {
    this.loadingProgress = true;
    this.messageService.getMessages(this.pageNumber, this.pageSize, this.container)
                      .subscribe((res: PaginatedResult<Message[]>) => {
                            this.messages = res.result;
                            this.pagination = res.pagination;
                            console.log(res);
                            this.loadingProgress = false;
                          });

    }

    pageChanged(event: any) {
      this.pageNumber = event.page;
      console.log('page number:', this.pageNumber);
      // get the next batch of messages
      this.loadMessages();
    }

    // I added this and removed the routing from the template, html
    navigateToProfile(container: string, recipientId: number, senderId: number) {
        console.log(`navigate to container: ${container}, senderId: ${senderId}, recipientId: ${recipientId}`);
        if (container === 'Outbox') {
           this.urlLink = '/members/' + recipientId;
        }
        if (container === 'Inbox') {
          this.urlLink = '/members/' + senderId;
        }
        this.router.navigate([this.urlLink], { queryParams: {tab: 3} });
    }

    deleteMessage() {
      console.log('message for delete:', this.messageTodeletId);
      this.messageService.deleteMessage(this.messageTodeletId).subscribe(res => {
        if (res['message'] === true) {
          this.hideChildModal();
          this.loadMessages();
        }
      });

    }
}
