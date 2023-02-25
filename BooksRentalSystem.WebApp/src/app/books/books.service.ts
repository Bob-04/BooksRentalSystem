import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { environment } from "src/environments/environment";
import { Observable } from "rxjs";
import { Book } from "./book.model";
import { Category } from "./category.model";

@Injectable({
  providedIn: "root",
})
export class BooksService {
  constructor(private http: HttpClient) {}

  getBooks(): Observable<Array<Book>> {
    return this.http.get<Array<Book>>(
      environment.publishersApiUrl + "bookads/"
    );
  }

  getUserBooks(): Observable<Array<Book>> {
    return this.http.get<Array<Book>>(
      environment.publishersApiUrl + "bookads/" + "mine"
    );
  }

  getBook(id: string): Observable<Book> {
    return this.http.get<Book>(environment.publishersApiUrl + "bookads/" + id);
  }

  createBook(book: Book): Observable<Book> {
    return this.http.post<Book>(
      environment.publishersApiUrl + "bookads/",
      book
    );
  }

  editBook(id: string, book: Book): Observable<Book> {
    return this.http.put<Book>(
      environment.publishersApiUrl + "bookads/" + id,
      book
    );
  }

  deleteBook(id: string) {
    return this.http.delete(environment.publishersApiUrl + "bookads/" + id);
  }

  getCategories(): Observable<Array<Category>> {
    return this.http.get<Array<Category>>(
      environment.publishersApiUrl + "bookads/" + "categories"
    );
  }

  search(queryString: string): Observable<Array<Book>> {
    return this.http.get<Array<Book>>(
      environment.publishersApiUrl + "bookads" + queryString
    );
  }

  sort(queryString: string): Observable<Array<Book>> {
    return this.http.get<Array<Book>>(
      environment.publishersApiUrl + "bookads" + queryString
    );
  }

  changeAvailability(id: string, available: boolean): Observable<boolean> {
    return this.http.put<boolean>(
      `${environment.publishersApiUrl}bookads/${id}/ChangeAvailability?available=${available}`,
      {}
    );
  }
}
