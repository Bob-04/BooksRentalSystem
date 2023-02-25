import { Component, OnInit } from "@angular/core";
import { FormGroup, FormBuilder } from "ngx-strongly-typed-forms";
import { Book } from "../book.model";
import { Validators } from "@angular/forms";
import { BooksService } from "../books.service";
import { ToastrService } from "ngx-toastr";
import { Category } from "../category.model";
import { Router } from "@angular/router";

@Component({
  selector: "app-create",
  templateUrl: "./create.component.html",
  styleUrls: ["./create.component.css"],
})
export class CreateComponent implements OnInit {
  bookForm: FormGroup<Book>;
  categories: Array<Category>;

  constructor(
    private fb: FormBuilder,
    private booksService: BooksService,
    public toastr: ToastrService,
    private router: Router
  ) {
    this.booksService.getCategories().subscribe((res) => {
      this.categories = res;
    });
  }

  ngOnInit(): void {
    this.bookForm = this.fb.group<Book>({
      title: [null, Validators.required],
      description: [null],
      imageUrl: [null],
      pricePerDay: [null, [Validators.required, Validators.min(0)]],
      author: [null, Validators.required],
      category: [null, Validators.required],
      pagesNumber: [null],
      language: [null],
      coverType: [null],
    });
  }

  create() {
    this.booksService.createBook(this.bookForm.value).subscribe((res) => {
      this.router.navigate(["books", "mine"]);
    });
  }
}
