import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { HeaderService, OpalFooterService } from '@opal20/domain-components';

import { Component } from '@angular/core';

@Component({
  selector: 'webinar-error-page',
  templateUrl: './webinar-error-page.component.html'
})
export class WebinarErrorPageComponent extends BasePageComponent {
  public errorTitle: string = '';
  public errorMessage: string = '';
  public showTitle: boolean = false;
  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private opalFooterService: OpalFooterService,
    private headerService: HeaderService
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.opalFooterService.hide();
    this.headerService.hide();

    const param: { errorCode: string } = this.getNavigateData<{ errorCode: string }>();
    switch (param.errorCode) {
      case 'unauthenticated-user':
        this.showTitle = false;
        this.errorTitle = 'Unauthenticated Error';
        this.errorMessage = 'Unauthorized: Access is denied due to invalid credentials.';
        break;
      case 'meeting-unavailable':
        this.showTitle = false;
        this.errorTitle = 'Meeting is currently unavailable';
        this.errorMessage = 'The meeting is currently unavailable. Please try again later.';
        break;
      default:
        this.showTitle = true;
        this.errorTitle = 'Internal Server Error';
        this.errorMessage =
          'The server encountered an internal error or misconfiguration and was unable to complete your request.' +
          '<br /><br />' +
          'Please contact the server administrator and inform them of the time the error occurred, and anything you might have done that may have caused the error.';
        break;
    }
  }
}
