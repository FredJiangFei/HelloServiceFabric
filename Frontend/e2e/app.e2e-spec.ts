import { AgileSoftwareWorkshopPage } from './app.po';

describe('agile-software-workshop App', () => {
  let page: AgileSoftwareWorkshopPage;

  beforeEach(() => {
    page = new AgileSoftwareWorkshopPage();
  });

  it('should display welcome message', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('Welcome to asw!');
  });
});
