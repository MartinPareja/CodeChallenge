import { Component, inject, OnInit, signal, viewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatNativeDateModule } from '@angular/material/core';
import { FormBuilder, ReactiveFormsModule, FormControl, FormGroup } from '@angular/forms';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';

interface RentalMetrics {
  totalRentals: number;
  totalCancellations: number;
  carsNotRented: number;
}

interface TopCarTypes {
  carType: string;
  utilizationPercentage: number;
}

interface TopCars {
  car: {
    carId: string;
    type: string;
    make: string;
    model: string;
    year: number;
  };
  rentalCount: number;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatGridListModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatInputModule,
    MatNativeDateModule,
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatSelectModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private http = inject(HttpClient);
  private fb = inject(FormBuilder);
  private apiUrl = environment.apiUrl;

  paginator = viewChild<MatPaginator>(MatPaginator);

  metrics = signal<RentalMetrics | null>(null);
  topCarTypes = signal<TopCarTypes[]>([]);
  topCars = signal<TopCars[]>([]);
  topCarsTotalCount = signal<number>(0);
  errorMessage = signal<string | null>(null);

  dateForm = this.fb.group({
    date: [new Date()]
  });

  dateRangeForm = this.fb.group({
    startDate: [new Date(2025, 6, 1)],
    endDate: [new Date()]
  });

  topCarsFilterForm = this.fb.group({
    make: [''],
    model: [''],
    type: [''],
    location: [''],
  });

  topCarsPageSize = 10;
  topCarsPageIndex = 0;

  topCarTypesColumns: string[] = ['carType', 'utilizationPercentage'];
  topCarsColumns: string[] = ['make', 'model', 'year', 'rentalCount'];

  get dateControl(): FormControl {
    return this.dateForm.get('date') as FormControl;
  }

  get startDateControl(): FormControl {
    return this.dateRangeForm.get('startDate') as FormControl;
  }

  get endDateControl(): FormControl {
    return this.dateRangeForm.get('endDate') as FormControl;
  }

  get makeControl(): FormControl {
    return this.topCarsFilterForm.get('make') as FormControl;
  }

  get modelControl(): FormControl {
    return this.topCarsFilterForm.get('model') as FormControl;
  }

  get typeControl(): FormControl {
    return this.topCarsFilterForm.get('type') as FormControl;
  }

  get locationControl(): FormControl {
    return this.topCarsFilterForm.get('location') as FormControl;
  }

  ngOnInit(): void {
    this.fetchDashboardMetrics();
    this.fetchTopCarTypes();
    this.fetchTopCars();

    this.dateControl.valueChanges.subscribe(() => {
      this.fetchDashboardMetrics();
    });

    this.dateRangeForm.valueChanges.subscribe(() => {
      this.fetchTopCarTypes();
    });

    this.topCarsFilterForm.valueChanges.subscribe(() => {
      this.topCarsPageIndex = 0;
      this.fetchTopCars();
    });
  }

  fetchDashboardMetrics(): void {
    this.errorMessage.set(null);
    const selectedDate = this.dateControl.value;
    const formattedDate = selectedDate ? this.formatDate(selectedDate) : this.formatDate(new Date());

    this.http.get<RentalMetrics>(`${this.apiUrl}/Rentals/metrics?date=${formattedDate}`).subscribe({
      next: data => this.metrics.set(data),
      error: err => this.handleError(err)
    });
  }

  fetchTopCarTypes(): void {
    const startDate = this.startDateControl.value;
    const endDate = this.endDateControl.value;
    const formattedStartDate = startDate ? this.formatDate(startDate) : this.formatDate(new Date(2025, 6, 1));
    const formattedEndDate = endDate ? this.formatDate(endDate) : this.formatDate(new Date());

    this.http.get<TopCarTypes[]>(`${this.apiUrl}/Cars/top-types?startDate=${formattedStartDate}&endDate=${formattedEndDate}`).subscribe({
      next: data => this.topCarTypes.set(data),
      error: err => this.handleError(err)
    });
  }

  fetchTopCars(): void {
    const filters = this.topCarsFilterForm.value;
    let params = new HttpParams()
      .set('limit', this.topCarsPageSize)
      .set('offset', this.topCarsPageIndex * this.topCarsPageSize);

    if (filters.make) {
      params = params.set('make', filters.make);
    }
    if (filters.model) {
      params = params.set('model', filters.model);
    }
    if (filters.type) {
      params = params.set('type', filters.type);
    }
    if (filters.location) {
      params = params.set('location', filters.location);
    }

    this.http.get<{ data: TopCars[], totalCount: number }>(`${this.apiUrl}/Cars/top`, { params }).subscribe({
      next: response => {
        this.topCars.set(response.data);
        this.topCarsTotalCount.set(response.totalCount);
      },
      error: err => this.handleError(err)
    });
  }

  handlePageEvent(e: PageEvent): void {
    this.topCarsPageSize = e.pageSize;
    this.topCarsPageIndex = e.pageIndex;
    this.fetchTopCars();
  }

  private formatDate(date: Date): string {
    const d = new Date(date);
    const year = d.getFullYear();
    const month = (d.getMonth() + 1).toString().padStart(2, '0');
    const day = d.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  private handleError(err: any): void {
    console.error(err);
    this.errorMessage.set('Could not load dashboard data. Please try again later.');
  }
}