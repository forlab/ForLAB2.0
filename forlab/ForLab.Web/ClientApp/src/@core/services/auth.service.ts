import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Router } from '@angular/router';
import { UserDetailsDto } from '../models/security/User';
import * as jwt_decode from 'jwt-decode';

@Injectable()
export class AuthService {

  user: any;

  // logged in user
  public loggedInUser$: BehaviorSubject<UserDetailsDto> = new BehaviorSubject<UserDetailsDto>(null);

  // loadingAction
  public loadingAction$: BehaviorSubject<Boolean> = new BehaviorSubject<Boolean>(false);

  constructor(private router: Router) {
    this.user = JSON.parse(localStorage.getItem('lab_user'));
    if (this.user != null) {
      this.loggedInUser$.next(this.user);
      this.loadingAction$.next(false);
    }
  }

  getTokenExpireTime(): number {

    let result = 0;

    let user = JSON.parse(localStorage.getItem('lab_user'));
    if (user) {
      var decoded = jwt_decode(String(user.token));
      result = decoded.exp;
    }

    return result;
  }


  // store user data after login succeffully
  updateStoredUserInfo(userData) {
    let user = JSON.parse(localStorage.getItem('lab_user'));
    user.firstName = userData && userData.firstName ? userData.firstName : null;
    user.lastName = userData && userData.lastName ? userData.lastName : null;
    user.phoneNumber = userData && userData.phoneNumber ? userData.phoneNumber : null;
    if (userData.personalImagePath) {
      user.personalImagePath = userData.personalImagePath;
    }
    localStorage.setItem('lab_user', JSON.stringify(user));

    this.loggedInUser$.next(user);
  }

  updateStoredUserRoles(roles: string[]) {
    let user = JSON.parse(localStorage.getItem('lab_user'));
    user.userRoles = roles;
    localStorage.setItem('lab_user', JSON.stringify(user));
    this.loggedInUser$.next(user);
  }

  refreshToken(token: string) {
    let user = JSON.parse(localStorage.getItem('lab_user'));
    user.token = 'Bearer ' + token;
    localStorage.setItem('lab_user', JSON.stringify(user));
    this.loggedInUser$.next(user);
  }

  // store user data after login succeffully
  storeUserDate(user) {

    // local storage store only string, so need to convert json data to string
    // JSON.parse(user), to return user to an normal object
    user.is_token_expired = false;
    user.token = 'Bearer ' + user.token;
    localStorage.setItem('lab_user', JSON.stringify(user));

    this.loggedInUser$.next(JSON.parse(localStorage.getItem('lab_user')));
  }


  // logout
  logout() {
    this.user = null;
    // localStorage.clear();
    localStorage.removeItem('lab_user');
    localStorage.removeItem('locationDto');
    this.router.navigate(['/authentication/signin']);
  }

  // load the data
  loadToken() {
    if (localStorage.getItem('user') && JSON.parse(localStorage.getItem('lab_user')).token) {
      this.user = JSON.parse(localStorage.getItem('lab_user'));
    }
  }


  // check the role
  roleMatch(allowedRoles: string[]): boolean {

    if (!localStorage.getItem('lab_user')) {
      this.router.navigate(['/authentication/signin']);
      return false;
    }

    let isMatch = false;
    let userRoles: string[] = JSON.parse(localStorage.getItem('lab_user')).userRoles;
    allowedRoles.forEach(elem => {
      if (userRoles.indexOf(elem) > -1) {
        isMatch = true;
        return false;
      }
    });

    return isMatch;

  }


  // loading action
  ActionLoading(val: boolean) {
    this.loadingAction$.next(Boolean(val));
  }

}
