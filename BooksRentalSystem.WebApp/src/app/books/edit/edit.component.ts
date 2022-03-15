import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Book } from '../book.model';
import { FormGroup, FormBuilder } from 'ngx-strongly-typed-forms';
import { Validators } from '@angular/forms';
import { BooksService } from '../books.service';
import { ToastrService } from 'ngx-toastr';
import { Category } from '../category.model';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.css']
})
export class EditComponent implements OnInit {
  id: string;
  book: Book;
  bookForm: FormGroup<Book>;
  categories: Array<Category>;
  constructor(private route: ActivatedRoute,
    private fb: FormBuilder,
    private booksService: BooksService,
    private router: Router,
    public toastr: ToastrService) {
    this.id = this.route.snapshot.paramMap.get('id');
    this.bookForm = this.fb.group<Book>({
      title: ["", Validators.required],
      description: [""],
      imageUrl: [""],
      pricePerDay: [0, [Validators.required, Validators.min(0)]],
      author: ["", Validators.required],
      category: [0, Validators.required],
      pagesNumber: [0],
      language: [""],
      cover: [0]
    })
  }

  async ngOnInit(): Promise<void> {
    await this.fetchCategories()
    this.fetchBook()
  }

  mapCategory(book: Book) {
    var category = this.categories.filter(x => x.id == book.category)[0]
    this.bookForm.patchValue({ category: category.id })
  }

  mapCoverType(book: Book) {
    var coverType = new Array({ id: 1, text: 'Hard cover' }, { id: 2, text: 'Paper cover' })
      .filter(x => x.id == book.cover)[0];
    this.bookForm.patchValue({ cover: coverType.id })
  }

  mapDropDownData(book: Book) {
    this.mapCategory(book);
    this.mapCoverType(book);
  }

  fetchBook() {
    this.booksService.getBook(this.id).subscribe(book => {
      this.bookForm = this.fb.group<Book>({
        title: [book.title, Validators.required],
        description: [book.description],
        imageUrl: [book.imageUrl],
        pricePerDay: [book.pricePerDay, [Validators.required, Validators.min(0)]],
        author: [book.author, Validators.required],
        category: [book.category, Validators.required],
        pagesNumber: [book.pagesNumber],
        language: [book.language],
        cover: [book.cover]
      })
      this.mapDropDownData(book);
      console.log(this.bookForm.value)
    })
  }

  edit() {
    this.booksService.editBook(this.id, this.bookForm.value).subscribe(res => {
      this.router.navigate(['books']);
      this.toastr.success("Success")
    })
  }

  fetchCategories() {
    this.booksService.getCategories().subscribe(res => {
      this.categories = res;
    })
  }

  get imageUrl() {
    return this.bookForm.get('imageUrl').value;
  }

}
