import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class SpinnerService {

  constructor(private spinner: NgxSpinnerService) { }

  loadSpinner() {
    // setup the spinner style and image
    this.spinner.show(undefined, {
      // type: 'line-scale-party',
      type: 'ball-elastic-dots',
      size: 'large',
      bdColor: 'rgba(255,255,255, 0.3)',
      color: 'grey'
    });
  }

  hideSpinner() {
    this.spinner.hide();
  }

}
