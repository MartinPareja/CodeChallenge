// src/app/pages/my-rentals/my-rentals.component.ts
// (Full file content)

import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { HttpParams } from '@angular/common/http';
import { Rental, RentalService } from '../../services/rental.service';

@Component({
  selector: 'app-my-rentals',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatPaginatorModule,
    DatePipe
  ],
  templateUrl: './my-rentals.component.html',
  styleUrl: './my-rentals.component.scss'
})
export class MyRentalsComponent implements OnInit {
  private rentalService = inject(RentalService);

  rentals = signal<Rental[]>([]);
  totalCount = signal<number>(0);
  errorMessage = signal<string | null>(null);

  pageSize = 10;
  pageIndex = 0;
  pageSizeOptions = [5, 10, 20];

  // Updated columns to display richer data
  displayedColumns: string[] = ['car', 'make', 'model', 'startDate', 'endDate', 'isCancelled'];

  ngOnInit(): void {
    this.fetchMyRentals();
  }

  fetchMyRentals(): void {
    this.errorMessage.set(null);
    const params = new HttpParams()
      .set('limit', this.pageSize)
      .set('offset', this.pageIndex * this.pageSize);

    this.rentalService.getMyRentals(params).subscribe({
      next: response => {
        this.rentals.set(response.data);
        this.totalCount.set(response.totalCount);
      },
      error: err => {
        console.error(err);
        this.errorMessage.set('Could not load rentals data. Please try again.');
      }
    });
  }

  handlePageEvent(e: PageEvent): void {
    this.pageSize = e.pageSize;
    this.pageIndex = e.pageIndex;
    this.fetchMyRentals();
  }
}