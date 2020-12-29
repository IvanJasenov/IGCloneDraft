import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model: any = {};
  registerForm: FormGroup;
  maxDate: Date;
  validationErrors: string[] = [];

  constructor(private accountService: AccountService, private router: Router, private alertify: AlertifyService) { }

  ngOnInit() {
    this.initializeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initializeForm() {
    this.registerForm = new FormGroup({
      username: new FormControl('', Validators.required),
      gender: new FormControl('male'),
      knownAs: new FormControl('', Validators.required),
      dateOfBirth: new FormControl('', Validators.required),
      city: new FormControl('', Validators.required),
      country: new FormControl('', Validators.required),
      password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
      confirmPassword: new FormControl('', [Validators.required, Validators.minLength(4)])
    }, this.passwordMatchValidator.bind(this));
  }

  // apply this to the whole registration form because I need access to two FormControl elements
  passwordMatchValidator(g: FormGroup) {
    // if not matched, return an anonymous object
    return g.get('password').value === g.get('confirmPassword').value ? null : {mismatched: true};
  }

  register() {
    console.log(' reactive register form values:', this.registerForm.value);
    console.log('register model:', this.registerForm.value);
    this.accountService.register(this.registerForm.value).subscribe(response => {
      console.log('register response:', response);
      this.router.navigate(['instagram-photos']); // now navigate to photos and commnets
    }, error => {
      console.log('register error:', error);
      this.validationErrors = error;
    });
  }

  cancel() {
    this.model = null;
    console.log('cancel register');
  }

}
