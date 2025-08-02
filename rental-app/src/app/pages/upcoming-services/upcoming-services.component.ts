// src/app/pages/upcoming-services/upcoming-services.component.ts
// (Full file content)

import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { FormBuilder, ReactiveFormsModule, FormControl } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CarServicesService, UpcomingServiceCar } from '../../services/car-services.service';

@Component({
  selector: 'app-upcoming-services',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    ReactiveFormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './upcoming-services.component.html',
  styleUrl: './upcoming-services.component.scss'
})
export class UpcomingServicesComponent implements OnInit {
  private carServicesService = inject(CarServicesService);
  private fb = inject(FormBuilder);

  carsForService = signal<UpcomingServiceCar[]>([]);
  // Removed 'mileage' and added 'location'
  displayedColumns: string[] = ['make', 'model', 'year', 'location', 'serviceDate'];
  errorMessage = signal<string | null>(null);

  dateForm = this.fb.group({
    date: [new Date()]
  });

  get dateControl(): FormControl {
    return this.dateForm.get('date') as FormControl;
  }

  ngOnInit(): void {
    this.fetchUpcomingServices();

    this.dateControl.valueChanges.subscribe(() => {
      this.fetchUpcomingServices();
    });
  }

  fetchUpcomingServices(): void {
    this.errorMessage.set(null);
    const selectedDate = this.dateControl.value;
    const formattedDate = selectedDate ? this.formatDate(selectedDate) : this.formatDate(new Date());

    this.carServicesService.getUpcomingServices(formattedDate).subscribe({
      next: data => this.carsForService.set(data),
      error: err => {
        console.error(err);
        this.errorMessage.set('Could not fetch upcoming services. Please try again.');
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
}