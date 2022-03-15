import { Component, OnInit } from '@angular/core';
import { BooksService } from '../books.service';
import { Book } from '../book.model';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit {
  books: Array<Book>;
  id: string;
  category = null;
  constructor(private booksService: BooksService) { }

  ngOnInit(): void {
    this.fetchBooks()
  }

  fetchBooks() {
    this.booksService.getBooks().subscribe(books => {
      this.books = books['bookAds'];
    })
  }

  assignBooks(event: { [x: string]: Book[]; }) {
    this.books = event['bookAds'];
  }

}
