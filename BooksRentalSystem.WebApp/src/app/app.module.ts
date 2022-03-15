import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthenticationModule } from './authentication/authentication.module';
import { BooksModule } from './books/books.module';
import { SharedModule } from './shared/shared.module';
import { PublishersModule } from './publishers/publishers.module'

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    SharedModule,
    AuthenticationModule,
    BooksModule,
    PublishersModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
