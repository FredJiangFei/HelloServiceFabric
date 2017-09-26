import { Injectable } from '@angular/core';

@Injectable()
export class AlertMessageService {
  public alerts: any = [];

  constructor() { }

  public alertErrorMessage(message: string) {
    this.insertMessage(message, 'danger');
  }

  public alertSuccessMessage(message: string) {
    this.insertMessage(message, 'success');
  }

  public alertWarningMessage(message: string) {
    this.insertMessage(message, 'warning');
  }

  public alertInfoMessage(message: string) {
    this.insertMessage(message, 'info');
  }

  private insertMessage(message: string, type: string) {
    const alert = {
      type: type,
      msg: message
    };

    const alerts = this.alerts;
    alerts.push(alert);

    setTimeout(function () {
      alerts.pop(alert);
    }, 3000);
  }
}
