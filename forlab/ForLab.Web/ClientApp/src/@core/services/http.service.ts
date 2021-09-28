import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { throwError } from 'rxjs';
// services
import { AuthService } from './auth.service';
import { LocationDto } from '../models/common/Location';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { map, catchError } from 'rxjs/operators';
import { QueryParamsDto, ResponseDto } from '../models/common/response';

@Injectable()
export class HttpService {

  constructor(
    private router: Router,
    private authService: AuthService,
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
      if (res?.value != null && res?.value != undefined) {
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
            user && user.token ? this.setTokenExpiriation(true) : this.authService.logout();
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
            user && user.token ? this.setTokenExpiriation(true) : this.authService.logout();
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
            user && user.token ? this.setTokenExpiriation(true) : this.authService.logout();
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
            user && user.token ? this.setTokenExpiriation(true) : this.authService.logout();
          } else if (err.status === 403) { // 403 (Forbidden)
            this.router.navigate(['/authentication/page403']);
          }
        }),
        map(res => res.body as ResponseDto)
      );

  }


  // Get Location info
  async GetCurrentTime() {

    // const url = 'https://api.timezonedb.com/v2.1/get-time-zone?key=YTCIS5WPHW5Z&format=json&by=zone&zone=Africa/Cairo';
    // const url = 'http://worldtimeapi.org/api/ip';
    const url = 'https://script.googleusercontent.com/macros/echo?user_content_key=pSSiY171tKcI8iyd-a81k_uWZGfziedpnhiXCCHb1McSFbwIC7_nrN6PjJYwJo2wCuDAbz_DmyBUh8tAoWWa2a1w-awrV3xcm5_BxDlH2jW0nuo2oDemN9CCS2h10ox_1xSncGQajx_ryfhECjZEnJ9GRkcRevgjTvo8Dc32iw_BLJPcPfRdVKhJT5HNzQuXEeN3QFwl2n0M6ZmO-h7C6eIqWsDnSrEd&lib=MwxUjRcLr2qLlnVOLh12wSNkqcO1Ikdrk';

    let time: number;
    const res = await this.http.get(url).toPromise();
    if (res) {
      const time = res;
    }
    return time;
  }

  ExportToExcel(url: string, params: QueryParamsDto[] = []) {

    // params
    let httpParams: HttpParams = this.ReturnParameterizedUrl(params);

    // headers
    const user = JSON.parse(localStorage.getItem('lab_user'));
    let headers: HttpHeaders = new HttpHeaders();
    headers.append(`Content-Type`, 'application/json');
    if (user && user.token) {
      headers = headers.append('Authorization', user.token);
    }

    return this.http.post(url, '', {
      observe: 'response',
      params: httpParams,
      headers, responseType: 'blob'
    })
      .pipe(
        catchError(err => {
          if (err.status <= 400 || err.status === 500) {
            // return _throw(new Error(String(err.status) + ' ' + err.statusText));
            const errSTR = JSON.stringify(err);
            const errJSON = JSON.parse(errSTR);
            return throwError(errJSON._body);
          } else if (err.status === 401) { // 401 (not authorized)
            user && user.token ? this.setTokenExpiriation(true) : this.authService.logout();
          } else if (err.status === 403) { // 403 (Forbidden)
            this.router.navigate(['/authentication/page403']);
          }
        })
      );
  }

  async GetLocationInfo() {

    if (localStorage.getItem('locationDto')) {
      return JSON.parse(localStorage.getItem('locationDto'));
    }

    // https://medium.com/@ipdata_co/what-is-the-best-commercial-ip-geolocation-api-d8195cda7027
    // const url = 'https://api.ipdata.co/?api-key=test';
    // const url = 'http://api.ipstack.com/197.39.65.31?access_key=07f7e996de19dfced1d3ef6d55addd0c';
    const url = 'https://ipapi.co/json/';
    // const url = 'http://ipinfo.io/json';

    let locationDto: LocationDto = new LocationDto();
    let data = await this.http.get(url).toPromise() as any;
    locationDto.IP = data.ip;
    locationDto.city = data.city;
    locationDto.countryName = data.country_name;
    locationDto.countryCode = data.country_code || data.country;
    locationDto.continentName = data.continent_name;
    locationDto.continentCode = data.continent_code;
    locationDto.callingCode = data.calling_code;
    locationDto.flag = data.flag;
    locationDto.latitude = data.latitude ? data.latitude : 0;
    locationDto.longitude = data.longitude ? data.longitude : 0;
    locationDto.postal = data.postal ? data.postal : null;
    locationDto.region = data.region;
    locationDto.regionCode = data.region_code;
    // locationDto.ASN = data.asn && data.asn.asn ? data.asn.asn : null; // ipdata
    locationDto.ASN = data.asn ? data.asn : null; // ipapi
    locationDto.RTL = data.languages && data.languages.rtl ? data.languages.rtl : false;


    // set in local storage
    localStorage.setItem('locationDto', JSON.stringify(locationDto));
    return locationDto;
  }

}
