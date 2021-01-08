import { BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

import { AuthService } from '@opal20/authentication';
import { Component } from '@angular/core';
import { LearnerRoutePaths } from '@opal20/domain-components';

@Component({
  selector: 'e-porfolio',
  template: `
    <iframe *ngIf="safeUrl" [src]="safeUrl" frameborder="0" (load)="onLoadIframe()"></iframe>
  `,
  styles: [
    `
      :host {
        height: 100%;
      }
      iframe {
        width: 100%;
        height: calc(100% + 40px);
      }
    `
  ]
})
export class EPortfolioPageComponent extends BasePageComponent {
  public safeUrl: SafeResourceUrl;
  private url: string;

  constructor(protected moduleFacadeService: ModuleFacadeService, private domSanitizer: DomSanitizer, private authService: AuthService) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    this.setupIframe();
    this.updateDeeplink(`learner/${LearnerRoutePaths.EPortfolio}`);
  }

  public onLoadIframe(): void {
    this.moduleFacadeService.globalSpinnerService.hide();
  }

  private setupIframe(): void {
    this.moduleFacadeService.globalSpinnerService.show();
    const userExtId = this.authService.User.extId;

    const ePortfolioUrl = AppGlobal.environment.ePortfolioUrl;
    this.url = `${ePortfolioUrl}?id=${userExtId}`;

    this.safeUrl = this.domSanitizer.bypassSecurityTrustResourceUrl(this.url);
  }
}
