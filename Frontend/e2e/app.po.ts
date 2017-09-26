import { browser, by, element } from 'protractor';

export class AgileSoftwareWorkshopPage {
  navigateTo() {
    return browser.get('/');
  }

  getParagraphText() {
    return element(by.css('asw-root h1')).getText();
  }
}
