import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'enumOptionlist'
})
export class EnumOptionListPipe implements PipeTransform {
  transform(value: any, args: any[]): any {
    let items: any[] = [];
    if (
      (args != undefined && (args.length > 0 && args[0] === false)) ||
      (args == undefined || args.length === 0)
    ) {
      items.push({ key: '', value: '---choose---' });
    }
    for (let key in value) {
      var isValueProperty = parseInt(key, 10) >= 0;
      if (!isValueProperty) continue;
      items.push({ key: Number(key), value: value[key] });
    }
    return items;
  }
}

// How to use it
// <option *ngFor="let option of (YourEnum | enumOptionlist: true)" [value]="option.key">{{
//   option.value
// }}</option>