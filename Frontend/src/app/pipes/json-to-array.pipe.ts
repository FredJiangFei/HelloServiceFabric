import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'jsonToArray'
})

export class JsonToArrayPipe implements PipeTransform {
  transform(value: any, args?: any): any {
    if (!value) {
      return value;
    }
    return JSON.parse(value);
  }
}
