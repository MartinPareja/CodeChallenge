// src/app/pages/rental-modify-dialog/rental-modify-dialog.component.ts

import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Rental, RentalService } from '../../services/rental.service';
import { CarSelectionDialogComponent } from '../car-selection-dialog/car-selection-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { Car } from '../../services/car-fleet.service';

@Component({
  selector: 'app-rental-modify-dialog',
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
    MatNativeDateModule,
    ReactiveFormsModule,
    DatePipe
  ],
  templateUrl: './rental-modify-dialog.component.html',
  styleUrl: './rental-modify-dialog.component.scss'
})
export class RentalModifyDialogComponent implements OnInit {
  private dialogRef = inject(MatDialogRef<RentalModifyDialogComponent>);
  private rentalService = inject(RentalService);
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);
  data: Rental = inject(MAT_DIALOG_DATA);

  modifyForm!: FormGroup;
  currentCar = signal<Car | null>(null);

  ngOnInit(): void {
    const car = this.data.car as Car;
    this.currentCar.set(car);
    this.modifyForm = this.fb.group({
      carId: [this.data.car.carId, Validators.required],
      newStartDate: [new Date(this.data.startDate), Validators.required],
      newEndDate: [new Date(this.data.endDate), Validators.required]
    });
  }

  onPickNewCar(): void {
    const dialogRef = this.dialog.open(CarSelectionDialogComponent, {
      width: '800px',
      data: {
        startDate: this.modifyForm.get('newStartDate')?.value,
        endDate: this.modifyForm.get('newEndDate')?.value
      }
    });

    dialogRef.afterClosed().subscribe((selectedCar: Car | null) => {
      if (selectedCar) {
        this.currentCar.set(selectedCar);
        this.modifyForm.get('carId')?.setValue(selectedCar.carId);
      }
    });
  }

  onSave(): void {
    if (this.modifyForm.valid) {
      const payload = {
        rentalId: this.data.rentalId,
        carId: this.modifyForm.get('carId')?.value,
        newStartDate: this.modifyForm.get('newStartDate')?.value.toISOString(),
        newEndDate: this.modifyForm.get('newEndDate')?.value.toISOString()
      };

      this.rentalService.modifyRental(this.data.rentalId, payload).subscribe({
        next: () => {
          this.dialogRef.close(true); // Close with a positive result
        },
        error: err => {
          console.error(err);
          let errorMessage = 'An error occurred. Please try again.';
          if (err.error && err.error.message) {
            errorMessage = err.error.message;
          } else if (err.error && err.error.errors) {
            errorMessage = Object.values(err.error.errors).flat().join(' ');
          }
          this.snackBar.open(errorMessage, 'Dismiss', { duration: 5000 });
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}