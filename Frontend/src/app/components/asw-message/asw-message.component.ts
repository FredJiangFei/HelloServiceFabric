import { Component } from '@angular/core';
import {AlertMessageService} from '../../services/alert-message.service';

@Component({
  selector: 'asw-message',
  templateUrl: './asw-message.component.html'
})

export class AswMessageComponent {
  constructor(public alertMessageService: AlertMessageService) { }
}
