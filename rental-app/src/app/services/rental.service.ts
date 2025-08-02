// src/app/services/rental.service.ts
// (Full file content)

import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { CarFleetResponse } from './car-fleet.service';

export interface Customer {
  fullName: string;
  street: string;
  city: string;
  countryCode: string;
}

export interface CustomerResponse {
  customerId: string;
}

export interface CreateRentalRequest {
  customerId: string;
  carId: string;
  startDate: string;
  endDate: string;
}

export interface RentalResponse {
  rentalId: string;
}

export interface Rental {
  rentalId: string;
  startDate: string;
  endDate: string;
  isCancelled: boolean;
  car: {
    carId: string;
    type: string;
    make: string;
    model: string;
    year: number;
  };
  customer: {
    customerId: string;
    fullName: string;
    address: {
      street: string;
      city: string;
      countryCode: string;
    };
  };
}

export interface MyRentalsResponse {
  data: Rental[];
  totalCount: number;
  limit: number;
  offset: number;
}

@Injectable({
  providedIn: 'root'
})
export class RentalService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  getAvailableCars(params: HttpParams): Observable<CarFleetResponse> {
    return this.http.get<CarFleetResponse>(`${this.apiUrl}/Rentals/availability`, { params });
  }

  createCustomer(customer: Customer): Observable<CustomerResponse> {
    return this.http.post<CustomerResponse>(`${this.apiUrl}/Customers`, customer);
  }

  createRental(rental: CreateRentalRequest): Observable<RentalResponse> {
    return this.http.post<RentalResponse>(`${this.apiUrl}/Rentals`, rental);
  }

  // Corrected method to fetch rentals for the current user
  getMyRentals(params: HttpParams): Observable<MyRentalsResponse> {
    return this.http.get<MyRentalsResponse>(`${this.apiUrl}/Rentals/user`, { params });
  }
}