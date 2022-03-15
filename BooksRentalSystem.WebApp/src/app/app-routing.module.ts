import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./authentication/authentication-routing.module').then(m => m.AuthenticationRoutingModule)
  },
  {
    path: 'books',
    loadChildren: () => import('./books/books-routing.module').then(m => m.BooksRoutingModule)
  },
  {
    path: 'publishers',
    loadChildren: () => import('./publishers/publishers-routing.module').then(m => m.PublishersRoutingModule)
  },
  {
    path: '',
    loadChildren: () => import('./shared/shared-routing.module').then(m => m.SharedRoutingModule)
  },
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
