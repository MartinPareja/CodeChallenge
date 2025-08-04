
import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { inject } from '@angular/core';
import { AuthService } from './core/auth.service';
import { Router } from '@angular/router';
import { UpcomingServicesComponent } from './pages/upcoming-services/upcoming-services.component';
import { CarFleetComponent } from './pages/car-fleet/car-fleet.component';
import { MyRentalsComponent } from './pages/my-rentals/my-rentals.component';

const authGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  if (authService.isLoggedIn()) {
    return true;
  }
  return router.parseUrl('/login');
};

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [authGuard]
  },
  {
    path: 'car-fleet',
    component: CarFleetComponent,
    canActivate: [authGuard]
  },
  {
    path: 'upcoming-services',
    component: UpcomingServicesComponent,
    canActivate: [authGuard]
  },
  {
    path: 'my-rentals',
    component: MyRentalsComponent,
    canActivate: [authGuard]
  },
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: '**', redirectTo: 'dashboard' }
];