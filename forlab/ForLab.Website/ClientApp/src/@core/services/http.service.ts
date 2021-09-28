import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
// services
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { map, catchError } from 'rxjs/operators';
import { QueryParamsDto, ResponseDto } from '../models/common/response';
import { throwError } from 'rxjs';

@Injectable()
export class HttpService {

  constructor(
    private router: Router,
    private http: HttpClient
  ) { }


  setTokenExpiriation(isExpire: boolean) {
    const user = JSON.parse(localStorage.getItem('lab_user'));
    user.is_token_expired = isExpire;
    localStorage.setItem('lab_user', JSON.stringify(user));
    this.router.navigate(['/authentication/locked']);
  }

  prepareRequestHeaders(containFiles: boolean = false) {
    let headers: HttpHeaders = new HttpHeaders();

    if (containFiles) {
      headers.append('Accept', 'application/json');
    } else {
      headers.append('Content-Type', 'application/json');
    }

    const user = JSON.parse(localStorage.getItem('lab_user'));
    if (user && user.token) {
      headers.append('Authorization', user.token);
      console.log(user.token)
    }

    return headers;
  }


  ReturnParameterizedUrl(params: QueryParamsDto[]): HttpParams {

    // params
    let httpParams: HttpParams = new HttpParams();
    if (!params) {
      return httpParams;
    }
    
    params.forEach(res => {
      if (res.value != null && res.value != undefined) {
        if (Array.isArray(res.value)) { // Incase you pass array of Ids 
          let arr = res.value as string[];
          httpParams = httpParams.append(res.key, JSON.stringify(arr.join(',')));
        } else if (Object.prototype.toString.call(res.value) === '[object Date]') {
          httpParams = httpParams.append(res.key, new Date(res.value).toISOString());
        } else if (typeof res.value == 'object') {
          Object.keys(res.value).forEach(k => {
            httpParams = httpParams.append(k, res.value[k]);
          });
        } else {
          httpParams = httpParams.append(res.key, res.value);
        }

      }
    })

    return httpParams;

  }

  // GET request
  GET(url: string, params: QueryParamsDto[] = []) {

    // params
    let httpParams: HttpParams = this.ReturnParameterizedUrl(params);

    // headers
    const user = JSON.parse(localStorage.getItem('lab_user'));
    let headers: HttpHeaders = new HttpHeaders();
    headers = headers.append('Content-Type', 'application/json');
    if (user && user.token) {
      headers = headers.append('Authorization', user.token);
    }

    return this.http.get(url, { observe: 'response', params: httpParams, headers })
      .pipe(
        catchError(err => {
          if (err.status <= 400 || err.status === 500) {
            const errSTR = JSON.stringify(err);
            const errJSON = JSON.parse(errSTR);
            return throwError(errJSON._body);
          } else if (err.status === 401) { // 401 (not authorized)
            // user && user.token ? this.setTokenExpiriation(true) : this.authService.logout();
          } else if (err.status === 403) { // 403 (Forbidden)
            this.router.navigate(['/authentication/page403']);
          }
        }),
        map(res => res.body as ResponseDto)
      );

  }


  // POST request
  POST(url: string, body: any = null, params: QueryParamsDto[] = [], containFiles: boolean = false) {

    // params
    let httpParams: HttpParams = this.ReturnParameterizedUrl(params);

    // headers
    const user = JSON.parse(localStorage.getItem('lab_user'));
    let headers: HttpHeaders = new HttpHeaders();
    headers.append(`${containFiles ? 'Accept' : 'Content-Type'}`, 'application/json');
    if (user && user.token) {
      headers = headers.append('Authorization', user.token);
    }

    return this.http.post(url, body, { observe: 'response', params: httpParams, headers })
      .pipe(
        catchError(err => {
          if (err.status <= 400 || err.status === 500) {
            const errSTR = JSON.stringify(err);
            const errJSON = JSON.parse(errSTR);
            return throwError(errJSON._body);
          } else if (err.status === 401) { // 401 (not authorized)
            // user && user.token ? this.setTokenExpiriation(true) : this.authService.logout();
          } else if (err.status === 403) { // 403 (Forbidden)
            this.router.navigate(['/authentication/page403']);
          }
        }),
        map(res => res.body as ResponseDto)
      );

  }

  // PUT request
  PUT(url: string, body: any = null, params: QueryParamsDto[] = [], containFiles: boolean = false) {

    // params
    let httpParams: HttpParams = this.ReturnParameterizedUrl(params);

    // headers
    const user = JSON.parse(localStorage.getItem('lab_user'));
    let headers: HttpHeaders = new HttpHeaders();
    headers.append(`${containFiles ? 'Accept' : 'Content-Type'}`, 'application/json');
    if (user && user.token) {
      headers = headers.append('Authorization', user.token);
    }

    return this.http.put(url, body, { observe: 'response', params: httpParams, headers })
      .pipe(
        catchError(err => {
          if (err.status <= 400 || err.status === 500) {
            const errSTR = JSON.stringify(err);
            const errJSON = JSON.parse(errSTR);
            return throwError(errJSON._body);
          } else if (err.status === 401) { // 401 (not authorized)
            // user && user.token ? this.setTokenExpiriation(true) : this.authService.logout();
          } else if (err.status === 403) { // 403 (Forbidden)
            this.router.navigate(['/authentication/page403']);
          }
        }),
        map(res => res.body as ResponseDto)
      );

  }


  // DELETE request
  DELETE(url: string, params: QueryParamsDto[] = []) {

    // params
    let httpParams: HttpParams = this.ReturnParameterizedUrl(params);

    // headers
    const user = JSON.parse(localStorage.getItem('lab_user'));
    let headers: HttpHeaders = new HttpHeaders();
    headers.append('Content-Type', 'application/json');
    if (user && user.token) {
      headers = headers.append('Authorization', user.token);
    }

    return this.http.delete(url, { observe: 'response', params: httpParams, headers })
      .pipe(
        catchError(err => {
          if (err.status <= 400 || err.status === 500) {
            const errSTR = JSON.stringify(err);
            const errJSON = JSON.parse(errSTR);
            return throwError(errJSON._body);
          } else if (err.status === 401) { // 401 (not authorized)
            // user && user.token ? this.setTokenExpiriation(true) : this.authService.logout();
          } else if (err.status === 403) { // 403 (Forbidden)
            this.router.navigate(['/authentication/page403']);
          }
        }),
        map(res => res.body as ResponseDto)
      );

  }

}
