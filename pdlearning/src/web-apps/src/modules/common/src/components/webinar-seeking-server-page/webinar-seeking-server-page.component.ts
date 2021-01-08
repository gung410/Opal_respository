import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { HeaderService, OpalFooterService } from '@opal20/domain-components';

import { CommonRoutePaths } from '../../common.config';
import { Component } from '@angular/core';
import { WebinarApiService } from '@opal20/domain-api';

@Component({
  selector: 'webinar-seeking-server-page',
  templateUrl: './webinar-seeking-server-page.component.html'
})
export class WebinarSeekingServerPageComponent extends BasePageComponent {
  public meetingId: string = '';
  public title: string = '';
  public source: string = '';
  private timer: number;

  constructor(
    public moduleFacadeService: ModuleFacadeService,
    private opalFooterService: OpalFooterService,
    private headerService: HeaderService,
    private webinarApiService: WebinarApiService
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.opalFooterService.hide();
    this.headerService.hide();
    this.moduleFacadeService.spinnerService.show();
    const param: { meetingId: string; source: string } = this.getNavigateData<{
      meetingId: string;
      source: string;
    }>();
    this.meetingId = param.meetingId;
    this.source = param.source;
    this.timer = (setInterval(() => {
      this.getJoinWebinarClick();
    }, 5000) as unknown) as number;
  }

  public getJoinWebinarClick(): void {
    this.webinarApiService.getJoinURL(this.meetingId, this.source, false).then(
      result => {
        if (result.isSuccess) {
          if (result.joinUrl !== window.location.href) {
            clearInterval(this.timer);
            window.location.replace(result.joinUrl);
          }
        }
      },
      () => {
        clearInterval(this.timer);
        this.moduleFacadeService.navigationService.navigateTo(CommonRoutePaths.WebinarError, { errorCode: 'meeting-unavailable' });
      }
    );
  }
}
