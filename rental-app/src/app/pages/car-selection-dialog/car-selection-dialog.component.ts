import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpParams } from '@angular/common/http';
import { RentalService } from '../../services/rental.service';
import { Car } from '../../services/car-fleet.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-car-selection-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatPaginatorModule,
    MatTableModule,
    ReactiveFormsModule,
    MatProgressSpinnerModule,
    DatePipe
  ],
  templateUrl: './car-selection-dialog.component.html',
  styleUrl: './car-selection-dialog.component.scss'
})
export class CarSelectionDialogComponent implements OnInit {
  private dialogRef = inject(MatDialogRef<CarSelectionDialogComponent>);
  private rentalService = inject(RentalService);
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);
  data: { startDate: Date, endDate: Date } = inject(MAT_DIALOG_DATA);

  searchForm!: FormGroup;
  availableCars = signal<Car[]>([]);
  totalCount = signal<number>(0);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string | null>(null);

  pageSize = 5;
  pageIndex = 0;
  pageSizeOptions = [5, 10, 20];
  displayedColumns: string[] = ['make', 'model', 'year', 'type', 'location', 'actions'];

  ngOnInit(): void {
    this.searchForm = this.fb.group({
      startDate: [this.data.startDate || null],
      endDate: [this.data.endDate || null],
      make: [''],
      model: [''],
      year: [''],
      location: [''],
      type: ['']
    });

    this.searchCars();
  }

  searchCars(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);
    let params = new HttpParams()
      .set('limit', this.pageSize)
      .set('offset', this.pageIndex * this.pageSize);

    const formValues = this.searchForm.value;

    if (formValues.startDate) {
      params = params.set('startDate', formValues.startDate.toISOString().split('T')[0]);
    }
    if (formValues.endDate) {
      params = params.set('endDate', formValues.endDate.toISOString().split('T')[0]);
    }
    if (formValues.make) params = params.set('make', formValues.make);
    if (formValues.model) params = params.set('model', formValues.model);
    if (formValues.year) params = params.set('year', formValues.year);
    if (formValues.location) params = params.set('location', formValues.location);
    if (formValues.type) params = params.set('type', formValues.type);

    this.rentalService.getAvailableCars(params).subscribe({
      next: response => {
        this.availableCars.set(response.data);
        this.totalCount.set(response.totalCount);
        this.isLoading.set(false);
      },
      error: err => {
        console.error(err);
        this.errorMessage.set('Could not load car data. Please try again.');
        this.isLoading.set(false);
        this.availableCars.set([]);
      }
    });
  }

  handlePageEvent(e: PageEvent): void {
    this.pageSize = e.pageSize;
    this.pageIndex = e.pageIndex;
    this.searchCars();
  }

  onSelectCar(car: Car): void {
    this.dialogRef.close(car);
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}