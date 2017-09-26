import {Pipe, PipeTransform} from '@angular/core';

@Pipe({
  name: 'enumToArray'
})

export class EnumToArrayPipe implements PipeTransform {

  transform(value: any, args?: any): any {
    switch (args) {
      case 'name':
        return this.getNames(value);
      case 'value':
        return this.getValues(value);
    }

    return this.getNamesAndValues(value);
  }

  getNamesAndValues<T extends number>(e: any) {
    return this.getNames(e).map(n => ({name: n, value: e[n] as T}));
  }

  getNames(e: any) {
    return this.getObjValues(e).filter(v => typeof v === 'string') as string[];
  }

  getValues<T extends number>(e: any) {
    return this.getObjValues(e).filter(v => typeof v === 'number') as T[];
  }

  private getObjValues(e: any): (number | string)[] {
    return Object.keys(e).map(k => e[k]);
  }
}
