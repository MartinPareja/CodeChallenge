// src/app/pages/car-fleet/car-fleet.component.ts
// (Full file content)

import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { FormBuilder, ReactiveFormsModule, FormControl, FormGroup, Validators, ValidationErrors, AbstractControl } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { Car, CarFleetService } from '../../services/car-fleet.service';
import { HttpParams } from '@angular/common/http';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { RentCarDialogComponent } from '../../shared/dialogs/rent-car-dialog/rent-car-dialog.component';
import { RentalService } from '../../services/rental.service';

// Custom validator to check if the end date is on or after the start date
export const dateRangeValidator = (control: AbstractControl): ValidationErrors | null => {
  const startDate = control.get('startDate')?.value;
  const endDate = control.get('endDate')?.value;
  if (startDate && endDate && startDate > endDate) {
    return { dateRangeInvalid: true };
  }
  return null;
};

@Component({
  selector: 'app-car-fleet',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    ReactiveFormsModule,
    MatIconModule,
    MatButtonModule,
    MatSlideToggleModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatDialogModule
  ],
  templateUrl: './car-fleet.component.html',
  styleUrl: './car-fleet.component.scss'
})
export class CarFleetComponent implements OnInit {
  private rentalService = inject(RentalService);
  private fb = inject(FormBuilder);
  private dialog = inject(MatDialog);

  carFleet = signal<Car[]>([]);
  totalCount = signal<number>(0);
  errorMessage = signal<string | null>(null);

  // Set default dates to today and tomorrow
  defaultStartDate = new Date();
  defaultEndDate = new Date();

  constructor() {
    this.defaultEndDate.setDate(this.defaultEndDate.getDate() + 1);

    this.dateFilterForm = this.fb.group({
      startDate: [this.defaultStartDate, Validators.required],
      endDate: [this.defaultEndDate, Validators.required],
    }, { validators: dateRangeValidator });
  }

  dateFilterForm: FormGroup;

  // Pagination state
  pageSize = 10;
  pageIndex = 0;
  pageSizeOptions = [5, 10, 20];

  displayedColumns: string[] = ['make', 'model', 'year', 'type', 'location', 'isAvailable', 'rent'];

  ngOnInit(): void {
    // We can remove the initial fetch here because it will be triggered by the valueChanges subscription
    this.dateFilterForm.valueChanges.subscribe(() => {
      this.pageIndex = 0;
      this.fetchAvailableCars();
    });

    // Trigger an initial fetch after form is ready to populate the table on load
    this.fetchAvailableCars();
  }

  fetchAvailableCars(): void {
    this.errorMessage.set(null);

    if (this.dateFilterForm.invalid) {
      this.errorMessage.set('Please select a valid date range.');
      this.carFleet.set([]);
      this.totalCount.set(0);
      return;
    }

    const filters = this.dateFilterForm.value;
    const startDate = filters.startDate ? this.formatDate(filters.startDate) : '';
    const endDate = filters.endDate ? this.formatDate(filters.endDate) : '';

    let params = new HttpParams()
      .set('limit', this.pageSize)
      .set('offset', this.pageIndex * this.pageSize)
      .set('startDate', startDate)
      .set('endDate', endDate)
      .set('isAvailable', 'true');

    this.rentalService.getAvailableCars(params).subscribe({
      next: response => {
        this.carFleet.set(response.data);
        this.totalCount.set(response.totalCount);
      },
      error: err => {
        console.error(err);
        this.errorMessage.set('Could not load car fleet data. Please try again.');
      }
    });
  }

  handlePageEvent(e: PageEvent): void {
    this.pageSize = e.pageSize;
    this.pageIndex = e.pageIndex;
    this.fetchAvailableCars();
  }

  openRentCarDialog(car: Car): void {
    const dialogRef = this.dialog.open(RentCarDialogComponent, {
      width: '500px',
      data: {
        car,
        startDate: this.startDateControl.value,
        endDate: this.endDateControl.value
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === 'success') {
        this.fetchAvailableCars();
      }
    });
  }

  private formatDate(date: Date): string {
    const d = new Date(date);
    const year = d.getFullYear();
    const month = (d.getMonth() + 1).toString().padStart(2, '0');
    const day = d.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  get startDateControl(): FormControl {
    return this.dateFilterForm.get('startDate') as FormControl;
  }
  get endDateControl(): FormControl {
    return this.dateFilterForm.get('endDate') as FormControl;
  }
}