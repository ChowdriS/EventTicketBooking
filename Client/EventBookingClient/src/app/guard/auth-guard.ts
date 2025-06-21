import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Auth } from '../services/Auth/auth';
import { Getrole } from '../misc/Token';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: Auth, private router: Router) {}

  canActivate(): boolean {
    const token = this.authService.getToken();

    if (token) {
      return true;
    }

    this.router.navigate(['/default']);
    return false;
  }
}

@Injectable({
  providedIn: 'root'
})
export class UserGuard implements CanActivate {
  constructor(private router: Router) {}
  canActivate(): boolean {
    const token = localStorage.getItem('token');
    var role = Getrole(token);
    if (role == 'User') {
      return true;
    }
    this.router.navigate(['/login']);
    return false;
  }
}
