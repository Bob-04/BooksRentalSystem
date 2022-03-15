import { Component, OnInit } from '@angular/core';
import { Book } from '../book.model';
import { BooksService } from '../books.service';

@Component({
  selector: 'app-publisher-books',
  templateUrl: './publisher-books.component.html',
  styleUrls: ['./publisher-books.component.css']
})
export class PublisherBooksComponent implements OnInit {
  books: Array<Book>;
  popUpOpen: boolean = false;
  id: string;
  constructor(private booksService: BooksService) { }

  ngOnInit(): void {
    this.popUpOpen = false
    this.fetchBooks()
  }

  fetchBooks() {
    this.booksService.getUserBooks().subscribe(books => {
      this.books = books['bookAds'];
    })
  }

  openModal(id: string) {
    this.popUpOpen = true;
    this.id = id;
  }

  cancelModal() {
    this.popUpOpen = false;
    this.id = null;
  }

  assignBooks(event: { [x: string]: Book[]; }) {
    this.books = event['bookAds'];
  }

  deleteBook() {
    this.booksService.deleteBook(this.id).subscribe(res => {
      this.popUpOpen = false;
      this.fetchBooks();
    })
  }

  changeAvailability(id: any) {
    this.booksService.changeAvailability(id).subscribe(res => {
      this.fetchBooks()
    })
  }

}
