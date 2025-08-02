// src/app/services/car-services.service.ts
// (Full file content)

import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

// The new, corrected interface to match the API response
export interface UpcomingServiceCar {
  carId: string;
  type: string;
  make: string;
  model: string;
  year: number;
  location: string;
  service: {
    serviceId: string;
    date: string;
  };
}

@Injectable({
  providedIn: 'root'
})
export class CarServicesService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  getUpcomingServices(date: string): Observable<UpcomingServiceCar[]> {
    // Corrected API URL
    return this.http.get<UpcomingServiceCar[]>(`${this.apiUrl}/Cars/upcoming-services?date=${date}`);
  }
}