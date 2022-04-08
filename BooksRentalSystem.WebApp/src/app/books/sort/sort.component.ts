import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Sort } from './sort.model';
import { FormGroup, FormBuilder } from 'ngx-strongly-typed-forms';
import { Validators } from '@angular/forms';
import { BooksService } from '../books.service';
import { Book } from '../book.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sort',
  templateUrl: './sort.component.html',
  styleUrls: ['./sort.component.css']
})
export class SortComponent implements OnInit {
  sortForm: FormGroup<Sort>;
  @Output('emitter') emitter = new EventEmitter<Array<Book>>();

  constructor(private fb: FormBuilder, private booksService: BooksService, private router: Router) { }

  ngOnInit(): void {
    this.sortForm = this.fb.group<Sort>({
      sortBy: ['', Validators.required],
      order: [''],
    })
  }

  sort() {
    this.booksService.sort(this.getQueryUrl()).subscribe(books => {
      this.emitter.emit(books)
    })
  }

  getQueryUrl() {
    const params = new URLSearchParams();
    const formValue = this.sortForm.value; // this.form should be a FormGroup
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
