import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { BooksService } from 'src/app/books/books.service';
import { Book } from 'src/app/books/book.model';

@Component({
  selector: 'app-pagination',
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.css']
})
export class PaginationComponent implements OnInit {

  @Input('queryString') queryString: string;
  @Output('emitter') emitter = new EventEmitter<Array<Book>>();
  constructor(private booksService: BooksService) { }

  ngOnInit(): void {
  }

  changePage() {
    this.booksService.search(this.queryString).subscribe(res => {
      this.emitter.emit(res);
    })
  }

}
