import { Component, HostBinding } from '@angular/core';

import { OpalFooterService } from '../../services/opal-footer.services';

@Component({
  selector: 'opal-footer',
  templateUrl: './opal-footer.component.html'
})
export class OpalFooterComponent {
  public appGlobal = AppGlobal;
  public isShowFooter: boolean = true;
  private releaseDate: string =
    (AppGlobal.user && AppGlobal.user.siteData && AppGlobal.user.siteData.releaseDate) || AppGlobal.environment.lastUpdateString || '';
  constructor(private opalFooterService: OpalFooterService) {
    this.opalFooterService.isShow.subscribe(show => (this.isShowFooter = show));
  }

  public get copyrightInfo(): { year: number; lastUpdateString: string } {
    return {
      year: new Date().getFullYear(),
      lastUpdateString: this.releaseDate
    };
  }

  @HostBinding('class.hide') get hide(): boolean {
    return !this.isShowFooter;
  }
}
