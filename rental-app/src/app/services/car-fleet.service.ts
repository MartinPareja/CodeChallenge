import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

export interface Car {
  carId: string;
  make: string;
  model: string;
  year: number;
  type: string;
  location: string;
  isAvailable: boolean;
}

export interface CarFleetResponse {
  data: Car[];
  totalCount: number;
  limit: number;
  offset: number;
}

@Injectable({
  providedIn: 'root'
})
export class CarFleetService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  getCarFleet(params: HttpParams): Observable<CarFleetResponse> {
    
    return this.http.get<CarFleetResponse>(`${this.apiUrl}/Rentals/availability`, { params });
  }
}