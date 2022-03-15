import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListComponent } from './list/list.component';
import { CreateComponent } from './create/create.component';
import { EditComponent } from './edit/edit.component';
import { ViewComponent } from './view/view.component';
import { BooksRoutingModule } from './books-routing.module';
import { SharedModule } from '../shared/shared.module';
import { SearchComponent } from './search/search.component';
import { SortComponent } from './sort/sort.component';
import { PublisherBooksComponent } from './publisher-books/publisher-books.component';

@NgModule({
  declarations: [ListComponent, CreateComponent, EditComponent, ViewComponent, SearchComponent, SortComponent, PublisherBooksComponent],
  imports: [
    CommonModule,
    SharedModule,
    BooksRoutingModule,
  ],
  exports: [ListComponent, CreateComponent, EditComponent, ViewComponent, SearchComponent, PublisherBooksComponent]
})
export class BooksModule { }
