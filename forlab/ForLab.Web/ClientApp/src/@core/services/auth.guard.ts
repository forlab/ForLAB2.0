
import { Injectable } from '@angular/core';
import {
  CanActivate, Router, ActivatedRouteSnapshot,
  RouterStateSnapshot, CanLoad, Route
} from '@angular/router';
import { AuthService } from './auth.service';


@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate, CanLoad {
  canLoad(route: Route): boolean {

    const user = JSON.parse(localStorage.getItem('lab_user'));
    if (user && user.token) {

      if (user.is_token_expired) {
        this.router.navigate(['/authentication/locked']);
        return false;

      } else {
        return true;
      }

    } else {
      this.router.navigate(['/authentication/signin']);
      return false;
    }
  }

  constructor(private router: Router, private authService: AuthService) { }

  canActivate(next: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {

    const user = JSON.parse(localStorage.getItem('lab_user'));
    if (user && user.token) {

      if (user.is_token_expired) {
        this.router.navigate(['/authentication/locked']);
        return false;

      } else {
        return true;
      }

    } else {
      this.router.navigate(['/authentication/signin']);
      return false;
    }

  }

}
