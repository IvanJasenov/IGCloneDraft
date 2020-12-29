import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-test-errors',
  templateUrl: './test-errors.component.html',
  styleUrls: ['./test-errors.component.css']
})
export class TestErrorsComponent implements OnInit {

  baseUrl = 'https://localhost:44308/api/';

  constructor(private http: HttpClient) { }

  ngOnInit() {
  }

  get404Error() {
    this.http.get(this.baseUrl + 'buggy/not-found').subscribe(res => {
      console.log('response:', res);
    }, error => console.log('error:', error));
  }

  get400Error() {
    this.http.get(this.baseUrl + 'buggy/bad-request').subscribe(res => {
      console.log('response:', res);
    }, error => console.log('error:', error));
  }

  get500Error() {
    this.http.get(this.baseUrl + 'buggy/server-error').subscribe(res => {
      console.log('response:', res);
    }, error => console.log('error:', error));
  }

  get401Error() {
    this.http.get(this.baseUrl + 'buggy/authError').subscribe(res => {
      console.log('response:', res);
    }, error => console.log('error:', error));
  }

  get400ValidationError() {
    this.http.post(this.baseUrl + 'account/register', {}).subscribe(res => {
      console.log('response:', res);
    }, error => console.log('error:', error));
  }

}
