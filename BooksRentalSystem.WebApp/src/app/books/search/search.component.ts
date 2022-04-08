import { Component, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder } from 'ngx-strongly-typed-forms';
import { Search } from './search.model';
import { BooksService } from '../books.service';
import { Category } from '../category.model';
import { EventEmitter } from '@angular/core';
import { Book } from '../book.model';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent implements OnInit {
  searchForm: FormGroup<Search>
  categories: Array<Category>;
  category = null;
  @Output('emitter') emitter = new EventEmitter<Array<Book>>();
  constructor(private fb: FormBuilder, private booksService: BooksService, private router: Router, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.searchForm = this.fb.group<Search>({
      title: [''],
      author: [''],
      publisher: [''],
      category: [this.category],
      minPricePerDay: [null],
      maxPricePerDay: [null],
    })
    this.route.queryParams.subscribe(params => {
      this.category = params['category'];
      this.searchForm.patchValue({ category: this.category });
      this.search();
    });
    this.booksService.getCategories().subscribe(res => {
      this.categories = res;
    })
  }

  search() {
    var queryString = this.getQueryUrl();
    this.booksService.search(queryString).subscribe(books => {
      this.emitter.emit(books);
    });
  }

  getQueryUrl() {
    const params = new URLSearchParams();
    const formValue = this.searchForm.value; // this.form should be a FormGroup
    for (const key in formValue) {
      if (!formValue[key]) {
        continue;
      }
      params.append(key, formValue[key]);
    }

    var query = params.toString()
    if (this.router.url.includes('mine')) {
      query = '/mine?' + params
    }
    else {
      query = '?' + params
    }
    return query;
  }

}
