import { Component, OnInit } from '@angular/core';
import { Book } from '../book.model';
import { BooksService } from '../books.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-view',
  templateUrl: './view.component.html',
  styleUrls: ['./view.component.css']
})
export class ViewComponent implements OnInit {
  book: Book;
  id: string;
  constructor(private booksService: BooksService, private route: ActivatedRoute) {
    this.id = this.route.snapshot.paramMap.get('id');
  }

  ngOnInit(): void {
    this.booksService.getBook(this.id).subscribe(book => {
      this.book = book;
    });
  }
}
