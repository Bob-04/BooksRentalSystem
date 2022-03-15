import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfileComponent } from './profile/profile.component';
import { SharedModule } from '../shared/shared.module';
import { PublishersRoutingModule } from './publishers-routing.module';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [ProfileComponent],
  imports: [
    CommonModule,
    SharedModule,
    PublishersRoutingModule,
    ReactiveFormsModule,
  ],
  exports: [ProfileComponent]
})
export class PublishersModule { }
