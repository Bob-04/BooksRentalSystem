import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from 'ngx-strongly-typed-forms';
import { Profile } from './profile.model';
import { Validators } from '@angular/forms';
import { ProfileService } from './profile.service';
import { PasswordChange } from './password.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  profileForm: FormGroup<Profile>;
  user: Profile;
  changePasswordForm: FormGroup<PasswordChange>
  id: string;

  constructor(private fb: FormBuilder, private profileService: ProfileService, private router: Router) {
    this.changePasswordForm = this.fb.group<PasswordChange>({
      currentPassword: ['', Validators.required],
      newPassword: ['', Validators.required],
    })
  }

  ngOnInit(): void {
    this.id = localStorage.getItem('publisherId');
    this.fetchUser();
  }

  fetchUser() {
    this.profileService.getPublisher(this.id).subscribe(user => {
      this.user = user;

      this.profileForm = this.fb.group<Profile>({
        name: [this.user.name, Validators.required],
        phoneNumber: [this.user.phoneNumber, Validators.required],
      });
    });
  }

  editProfile() {
    this.profileService.editPublisher(this.id, this.profileForm.value).subscribe(res => {
      this.router.navigate(['books'])
    })
  }

  changePassword() {
    this.profileService.changePassword(this.changePasswordForm.value).subscribe(res => {
      localStorage.clear()
      window.location.reload()
      this.router.navigate(['auth']);
    })
  }

}
