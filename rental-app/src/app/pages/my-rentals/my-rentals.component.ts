import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { HttpParams } from '@angular/common/http';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Rental, RentalService } from '../../services/rental.service';
import { RentalModifyDialogComponent } from '../rental-modify-dialog/rental-modify-dialog.component';

@Component({
  selector: 'app-my-rentals',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    DatePipe
  ],
  templateUrl: './my-rentals.component.html',
  styleUrl: './my-rentals.component.scss'
})
export class MyRentalsComponent implements OnInit {
  private rentalService = inject(RentalService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  rentals = signal<Rental[]>([]);
  totalCount = signal<number>(0);
  errorMessage = signal<string | null>(null);
  isLoading = signal<boolean>(false);

  pageSize = 10;
  pageIndex = 0;
  pageSizeOptions = [5, 10, 20];

  displayedColumns: string[] = ['make', 'model', 'startDate', 'endDate', 'isCancelled', 'actions'];

  ngOnInit(): void {
    this.fetchMyRentals();
  }

  fetchMyRentals(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);
    const params = new HttpParams()
      .set('limit', this.pageSize)
      .set('offset', this.pageIndex * this.pageSize);

    this.rentalService.getMyRentals(params).subscribe({
      next: response => {
        this.rentals.set(response.data);
        this.totalCount.set(response.totalCount);
        this.isLoading.set(false);
      },
      error: err => {
        console.error(err);
        this.errorMessage.set('Could not load rentals data. Please try again.');
        this.isLoading.set(false);
      }
    });
  }

  handlePageEvent(e: PageEvent): void {
    this.pageSize = e.pageSize;
    this.pageIndex = e.pageIndex;
    this.fetchMyRentals();
  }

  onModifyRental(rental: Rental): void {
    const dialogRef = this.dialog.open(RentalModifyDialogComponent, {
      width: '600px',
      data: rental
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.snackBar.open('Rental modified successfully!', 'Dismiss', {
          duration: 3000
        });
        this.fetchMyRentals();
      }
    });
  }

  onCancelRental(rentalId: string): void {
    if (confirm('Are you sure you want to cancel this rental?')) {
      this.rentalService.cancelRental(rentalId).subscribe({
        next: () => {
          this.snackBar.open('Rental cancelled successfully!', 'Dismiss', {
            duration: 3000
          });
          this.fetchMyRentals();
        },
        error: err => {
          console.error(err);
          this.snackBar.open('Could not cancel rental. Please try again.', 'Dismiss', {
            duration: 3000
          });
        }
      });
    }
  }
}