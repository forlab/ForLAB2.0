import { OnDestroy, Injectable } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Injector, ChangeDetectorRef } from "@angular/core";
import { Router, ActivatedRoute } from '@angular/router';
import { Subject } from 'rxjs';
// Services
import { HttpService } from './http.service';
// Lib
import * as numeral from 'numeral';
import * as FileSaver from 'file-saver';
import { AlertService } from './alert.service';
import { QueryParamsDto } from '../models/common/response';

export class ServiceLocator {
  static injector: Injector;
}

@Injectable()
export class BaseService implements OnDestroy {

  loggedInUser: any;
  modalReference: any;
  loading = false;
  ngUnsubscribe = new Subject<void>();

  //#region FileExtension variables
  imageExtensions: string[] = ['png', 'jpg', 'jpeg', 'gif'];
  fileExtIcons: FileExtIcon[] = [
    { iconPath: './assets/img/files/doc.svg', supportedExt: ['doc', 'txt', 'docx'] },
    { iconPath: './assets/img/files/zip.svg', supportedExt: ['zip', 'rar'] },
    { iconPath: './assets/img/files/pdf.svg', supportedExt: ['pdf'] },
    { iconPath: './assets/img/files/css.svg', supportedExt: ['css'] },
    { iconPath: './assets/img/files/javascript.svg', supportedExt: ['js'] },
    { iconPath: './assets/img/files/xml.svg', supportedExt: ['xml'] },
    { iconPath: './assets/img/files/mp4.svg', supportedExt: ['mp4'] },
    { iconPath: './assets/img/files/html.svg', supportedExt: ['html'] },
    { iconPath: './assets/img/files/csv.svg', supportedExt: ['csv'] },
    { iconPath: './assets/img/files/jpg.svg', supportedExt: ['png', 'jpg', 'jpeg', 'gif'] },
  ];
  //#endregion

  //#region Services
  public alertService: AlertService;
  public datepipe: DatePipe;
  public httpService: HttpService;
  public router: Router;
  public activatedRoute: ActivatedRoute;
  public _ref: ChangeDetectorRef;
  //#endregion


  constructor(public injector: Injector) {
    this.alertService = this.injector.get(AlertService);
    this.datepipe = this.injector.get(DatePipe);
    this.httpService = this.injector.get(HttpService);
    this.router = this.injector.get(Router);
    this.activatedRoute = this.injector.get(ActivatedRoute);
    this._ref = this.injector.get(ChangeDetectorRef);

    this.loggedInUser = JSON.parse(localStorage.getItem('lab_user'));
  }

  detectChanges() {
    // Programmatically run change detection to fix issue in Safari
    setTimeout(() => {
      this._ref.detectChanges();
    }, 500);
  }

  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  //#region Validation Methods
  isEmpty(str) {
    return (!str || 0 === str.length);
  }

  isBlank(str) {
    return (!str || /^\s*$/.test(str));
  }
  //#endregion

  //#region Formatting Methods
  roundNumber(num: number): number {
    if (num) {
      return Number(num.toFixed(2));
    }
    return 0;
  }

  formatInt(v, displayDecimal: boolean = true) {
    if (v) {
      return displayDecimal ? numeral(v).format('0,0.00') : numeral(v).format('0,0');
    } else {
      return 0;
    }
  }

  formatPrice(p, currency: string = '$'): string {
    if (p) {
      if (currency.toLowerCase() == 'usd' || currency == '$') {
        return numeral(p).format(`$0,0.00`);
      }
      return numeral(p).format(`0,0.00`) + ' ' + currency.toUpperCase();
    } else {
      return '0';
    }
  }

  formatPriceWithoutCurrency(p) {
    if (p) {
      return numeral(p).format(`0,0.00`)
    } else {
      return '0';
    }
  }

  formatDate(date: string) {
    if (date) {
      return this.datepipe.transform(new Date(date), 'd MMM y');
    } else {
      return '-';
    }
  }


  formatMonthYear(date: string) {
    if (date) {
      return this.datepipe.transform(new Date(date), 'MMM y');
    } else {
      return '-';
    }
  }

  formatMonthDay(date: string) {
    if (date) {
      return this.datepipe.transform(new Date(date), 'd MMMM');
    } else {
      return '-';
    }
  }

  formatDateTime(date: string) {
    if (date) {
      return this.datepipe.transform(new Date(date), 'd MMM y, hh:mm a');
    } else {
      return '-';
    }
  }

  formatAddress(address) {
    if (address) {
      return `${address.street ? address.street : '-'}  ${address.suburb ? address.suburb : '-'}  ${address.state ? address.state : '-'}`;
    } else {
      return '-';
    }
  }

  getReadableFileSize(sizeInBytes: number) {
    var _size = sizeInBytes;
    var fSExt = new Array('Bytes', 'KB', 'MB', 'GB'),
      i = 0;
    while (_size > 900) { _size /= 1024; i++; }
    var exactSize = (Math.round(_size * 100) / 100) + ' ' + fSExt[i];
    return exactSize;
  }

  extractContent(s) {
    var span = document.createElement('span');
    span.innerHTML = s;
    return span.textContent || span.innerText;
  }

  //#endregion



  //#region Files Methods
  getFileNameFromURL(url: string): string {
    return url.substr(url.lastIndexOf('\\') + 1);
  }

  getFileExtFromURL(url: string): string {
    return url.substr(url.lastIndexOf('.') + 1).toLowerCase();
  }

  isImage(url: string): boolean {
    const ext = this.getFileExtFromURL(url);
    return this.imageExtensions.includes(ext);
  }

  getIconForFileType(ext: string): string {
    let result = './assets/img/files/doc.svg';
    this.fileExtIcons.forEach(r => {
      if (r.supportedExt.includes(ext))
        result = r.iconPath;
    });

    return result;
  }

  formatAttachmentName(attachemntName: string, charsCount: number = 25): string {
    if (!attachemntName) {
      return '-';
    }
    let name = attachemntName.substr(0, attachemntName.lastIndexOf('.'));
    if (name.length > charsCount) {
      let result = attachemntName.substr(0, charsCount - 1) + '...';
      result += attachemntName.substr(attachemntName.lastIndexOf('.')).toLowerCase();
      return result;
    }
    return attachemntName;
  }
  //#endregion



  groupBy(array, key) {

    return array.reduce((objectsByKeyValue, obj) => {
      const value = obj[key];
      objectsByKeyValue[value] = (objectsByKeyValue[value] || []).concat(obj);
      return objectsByKeyValue;
    }, {});

    /***
     * to loop in object
     * *ngFor="let record of defaultRoleActions | keyvalue"
     * {{ record.key }}
     * {{ record.value }}
     */

  }

  public DownloadFile(url: string): void {
    var name = url.substr(url.lastIndexOf('\\') + 1);
    FileSaver.saveAs(url, name);
  }

  getFilterParamsDtos(filterDto: any, skippedParams: string[] = []): QueryParamsDto[] {
    let skipped: string[] = ['pageSize', 'pageIndex', 'applySort', 'sortProperty', 'isAscending'].concat(skippedParams);
    let result: QueryParamsDto[] = [];

    if (filterDto != null) {
      Object.keys(filterDto).forEach(key => {
        if (!skipped.includes(key) && filterDto[key] != null && filterDto[key] != undefined) {
          if (Array.isArray(filterDto[key])) { // Incase you pass array of Ids
            let arr = filterDto[key] as string[];
            result.push({ key: key, value: arr.join(',') });
          } else if (typeof filterDto[key] == 'object') {
            result.push({ key: key, value: new Date(filterDto[key]).toISOString() });
          } else {
            result.push({ key: key, value: filterDto[key] });
          }
        }
      });
    }

    return result;
  }

  convertEnumToList(data: any): KeyValueObject[] {
    return Object.keys(data).map(key => ({ id: data[key], name: key }))
  }

}

class FileExtIcon {
  iconPath: string = null;
  supportedExt: string[] = [];
}

class KeyValueObject {
  id: number;
  name: string;
}