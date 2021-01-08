import { AppInfoService, BasePageComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Injector } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { LearnerRoutePaths, NavigationMenuService, OpalFooterService } from '@opal20/domain-components';
import { PdPlanIframeAction, PdPlanIframeMessage } from '../models/pdplan-communication.model';

import { PdPlanCommunicationService } from '../services/pdplan-communication.service';
import { filter } from 'rxjs/operators';
import { fromEvent } from 'rxjs';

@Component({
  selector: 'learner-pdplan-page',
  template: `
    <iframe *ngIf="safeUrl" [id]="iframeId" [src]="safeUrl" frameborder="0" (load)="onLoadIframe()"></iframe>
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
  ],
  providers: [PdPlanCommunicationService]
})
export class LearnerPdPlanPageComponent extends BasePageComponent {
  public safeUrl: SafeResourceUrl;
  public iframeId: string = 'pdplaniframe';
  private url: string;
  private pdplanOrigin = '*';
  private extendUrl: string = '';

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private domSanitizer: DomSanitizer,
    private appInfoService: AppInfoService,
    private pdplanCommunicationService: PdPlanCommunicationService,
    private opalFooterService: OpalFooterService,
    private navigationMenuService: NavigationMenuService,
    protected injector: Injector
  ) {
    super(moduleFacadeService);
  }

  public onInit(): void {
    const routingParameters: {
      tab: string;
    } = this.getNavigateData() || {
      tab: undefined
    };
    this.extendUrl = routingParameters.tab !== undefined ? `/${routingParameters.tab}` : '';
    this.opalFooterService.hide();
    this.setupIframe();
    this.updateDeeplink(`learner/${LearnerRoutePaths.PdPlan}${this.extendUrl}`);
  }

  public onDestroy(): void {
    this.opalFooterService.show();
  }

  public onLoadIframe(): void {
    this.moduleFacadeService.globalSpinnerService.hide();
    const iframe: HTMLIFrameElement = document.getElementById(this.iframeId) as HTMLIFrameElement;
    if (!iframe || !iframe.contentWindow) {
      return;
    }
    const iframeWindow = iframe.contentWindow;

    this.pdplanCommunicationService.iframeWindow = iframeWindow;

    this.sendAccessTokenToIframe();

    fromEvent(window, 'message')
      .pipe(
        this.untilDestroy(),
        filter((event: MessageEvent) => event.origin === this.pdplanOrigin)
      )
      .subscribe(event => {
        const iframeMessage = this.pdplanCommunicationService.getIframeMessage(event);
        this.handleIframeMessage(iframeMessage);
      });
  }

  private setupIframe(): void {
    this.moduleFacadeService.globalSpinnerService.show();
    const pdpmUrl = AppGlobal.environment.pdplanUrl;
    this.pdplanOrigin = this.getOriginFromUrl(pdpmUrl);
    this.url = `${pdpmUrl}/mobile-mpj-module${this.extendUrl}`;

    this.safeUrl = this.domSanitizer.bypassSecurityTrustResourceUrl(this.url);
  }

  private handleIframeMessage(iframeMessage: PdPlanIframeMessage): void {
    if (!iframeMessage) {
      return;
    }
    switch (iframeMessage.action) {
      case PdPlanIframeAction.WEBVIEW_READY:
        this.sendAccessTokenToIframe();
        break;
      case PdPlanIframeAction.CLICKED_OPEN_PDCATALOGUE:
        const catalogueMenu = this.navigationMenuService.items.find(item => item.id === LearnerRoutePaths.Catalogue);
        this.navigationMenuService.onMenuItemClick(
          (menuId, parameters, skipLocationChange) =>
            this.moduleFacadeService.navigationService.navigateTo(menuId, parameters, skipLocationChange),
          catalogueMenu
        );
        break;
      default:
        break;
    }
  }

  private sendAccessTokenToIframe(): void {
    const accessToken = this.appInfoService.getAccessToken();
    this.pdplanCommunicationService.sendAccessToken(accessToken);
  }

  private getOriginFromUrl(url: string): string {
    if (!url) {
      return '';
    }
    const pathArray = url.split('/');
    const protocol = pathArray[0];
    const host = pathArray[2];
    const origin = protocol + '//' + host;
    return origin;
  }
}
