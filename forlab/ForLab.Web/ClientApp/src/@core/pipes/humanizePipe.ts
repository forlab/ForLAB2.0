import { Pipe } from '@angular/core';

@Pipe({
  name: 'humanize'
})
export class HumanizePipe {
  transform(value: string) {
    if ((typeof value) !== 'string') {
      return value;
    }
    value = value.split(/(?=[A-Z])/).join(' ');
    value = value[0].toUpperCase() + value.slice(1);
    return value;
  }
}

// How to use it
// <h1>{{ 'yourText' | humanize}}</h1> result: Your Text