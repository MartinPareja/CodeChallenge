import { Component, inject, Inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormBuilder, ReactiveFormsModule, Validators, FormGroup } from '@angular/forms';
import { RentalService, CreateRentalRequest, Customer } from '../../../services/rental.service';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { switchMap } from 'rxjs/operators';
import { Car } from '../../../services/car-fleet.service';

export interface DialogData {
  car: Car;
  startDate: Date;
  endDate: Date;
}

@Component({
  selector: 'app-rent-car-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressBarModule,
    MatSnackBarModule
  ],
  templateUrl: './rent-car-dialog.component.html',
  styleUrl: './rent-car-dialog.component.scss'
})
export class RentCarDialogComponent {
  private dialogRef = inject(MatDialogRef<RentCarDialogComponent>);
  private fb = inject(FormBuilder);
  private rentalService = inject(RentalService);
  private snackBar = inject(MatSnackBar);

  customerForm: FormGroup;
  loading = signal<boolean>(false);

  constructor(@Inject(MAT_DIALOG_DATA) public data: DialogData) {
    this.customerForm = this.fb.group({
      fullName: ['', Validators.required],
      street: ['', Validators.required],
      city: ['', Validators.required],
      countryCode: ['', Validators.required]
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSubmit(): void {
    if (this.customerForm.valid) {
      this.loading.set(true);
      const customer: Customer = this.customerForm.value;

      this.rentalService.createCustomer(customer).pipe(
        switchMap(customerResponse => {
          const rentalRequest: CreateRentalRequest = {
            customerId: customerResponse.customerId,
            carId: this.data.car.carId,
            startDate: this.formatDate(this.data.startDate),
            endDate: this.formatDate(this.data.endDate),
          };
          return this.rentalService.createRental(rentalRequest);
        })
      ).subscribe({
        next: () => {
          this.snackBar.open('Rental created successfully!', 'Dismiss', {
            duration: 3000,
          });
          this.dialogRef.close('success');
        },
        error: err => {
          console.error('Rental creation failed', err);
          this.snackBar.open('Rental creation failed. Please try again.', 'Dismiss', {
            duration: 3000,
          });
          this.loading.set(false);
        }
      });
    }
  }

  private formatDate(date: Date): string {
    const d = new Date(date);
    const year = d.getFullYear();
    const month = (d.getMonth() + 1).toString().padStart(2, '0');
    const day = d.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}T00:00:00.000Z`;
  }
}