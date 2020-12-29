import { Component, OnInit, ViewChild } from '@angular/core';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css']
})
export class AdminPanelComponent implements OnInit {

  @ViewChild('adminTabs', {static: false}) adminTabs: TabsetComponent;
  tabSelected: string;
  loadUsers: boolean;
  loadPhotos: boolean;
  constructor() { }

  ngOnInit() {
  }

  onSelect(data: TabDirective): void {
    this.tabSelected = data.heading;
    console.log('selected tab:', this.tabSelected);
    if (this.tabSelected === 'Photo Management') {
      this.loadPhotos = true;
    }
  }

}
