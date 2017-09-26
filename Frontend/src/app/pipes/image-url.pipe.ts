import { Pipe, PipeTransform } from '@angular/core';
import {environment} from '../../environments/environment';

@Pipe({
  name: 'imageUrl'
})

export class ImageUrlPipe implements PipeTransform {
  transform(value: any, args?: any): any {
    return environment.API_URL + 'getImage/' + value;
  }
}
