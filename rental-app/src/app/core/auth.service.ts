// src/app/core/auth.service.ts
// (Full file content)

import { Injectable, inject, signal, PLATFORM_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { tap } from 'rxjs';
import { Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';

// Defined interfaces directly in this file
export interface UserCredential {
  username: string;
  password?: string;
}

export interface AuthResponse {
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private platformId = inject(PLATFORM_ID);
  private apiUrl = environment.apiUrl;
  private readonly TOKEN_KEY = 'auth_token';

  isLoggedIn = signal<boolean>(this.hasToken());

  login(credentials: UserCredential) {
    // Corrected API endpoint
    return this.http.post<AuthResponse>(`${this.apiUrl}/users/login`, credentials).pipe(
      tap(response => {
        if (response.token) {
          if (isPlatformBrowser(this.platformId)) {
            localStorage.setItem(this.TOKEN_KEY, response.token);
          }
          this.isLoggedIn.set(true);
          this.router.navigate(['/dashboard']);
        }
      })
    );
  }

  register(credentials: UserCredential) {
    // Corrected API endpoint (assuming it follows the same pattern)
    return this.http.post<AuthResponse>(`${this.apiUrl}/users/register`, credentials);
  }

  logout() {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem(this.TOKEN_KEY);
    }
    this.isLoggedIn.set(false);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem(this.TOKEN_KEY);
    }
    return null;
  }

  private hasToken(): boolean {
    if (isPlatformBrowser(this.platformId)) {
      return !!this.getToken();
    }
    return false;
  }
}