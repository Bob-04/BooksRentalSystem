import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ListComponent } from './list/list.component';
import { CreateComponent } from './create/create.component';
import { EditComponent } from './edit/edit.component';
import { ViewComponent } from './view/view.component';
import {
  AuthGuardService as AuthGuard
} from '../shared/auth-guard.service';
import { PublisherBooksComponent } from './publisher-books/publisher-books.component';

const routes: Routes = [
  { path: '', component: ListComponent },
  { path: 'mine', component: PublisherBooksComponent, canActivate: [AuthGuard] },
  { path: 'create', component: CreateComponent, canActivate: [AuthGuard] },
  { path: ':id', component: ViewComponent },
  { path: ':id/edit', component: EditComponent, canActivate: [AuthGuard] },
];

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})

export class BooksRoutingModule { } 