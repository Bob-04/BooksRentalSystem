import { Component, OnInit } from '@angular/core';
import { Category } from 'src/app/books/category.model';
import { BooksService } from 'src/app/books/books.service';
import { Router } from '@angular/router';
import { StatisticsService } from '../statistics/statistics.service';
import { Statistics } from '../statistics/statistics.model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  categories: Array<Category>;
  statistics: Statistics;

  constructor(
    private booksService: BooksService,
    private statisticsService: StatisticsService,
    private router: Router) { }

  ngOnInit(): void {
    this.booksService.getCategories().subscribe(res => {
      this.categories = res;
    });

    this.statisticsService.getStatistics().subscribe(res => {
      this.statistics = res;
    });
  }

  goToBooks(id: number) {
    this.router.navigate(['books'], { queryParams: { category: id } });
  }
}
