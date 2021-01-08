import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

import { AppInfoService } from '../app-info/app-info.service';

@Component({
  selector: 'amazon-cloudfront-iframe',
  template: `
    <iframe #cloudfrontIframe [src]="cloudfrontSigninUrl" style="display:none;" (load)="onIframeLoaded()"></iframe>
  `
})
export class AmazonCloudfrontIframeComponent implements OnInit {
  public cloudfrontSigninUrl: SafeResourceUrl = this.domSanitizer.bypassSecurityTrustResourceUrl('about:blank');
  @ViewChild('cloudfrontIframe', { static: false })
  private cloudfrontIframe: ElementRef;

  constructor(private appInfoService: AppInfoService, private domSanitizer: DomSanitizer) {}

  public ngOnInit(): void {
    this.cloudfrontSigninUrl = this.domSanitizer.bypassSecurityTrustResourceUrl(
      `${AppGlobal.environment.appUrl}/cloudfront-cookies-signer.html`
    );
  }

  public onIframeLoaded(): void {
    if (!this.cloudfrontIframe) {
      return;
    }

    const iframe: HTMLIFrameElement = this.cloudfrontIframe.nativeElement as HTMLIFrameElement;
    const accessToken: string = this.appInfoService.getAccessToken();

    if (!accessToken) {
      return;
    }

    iframe.contentWindow.postMessage(
      {
        accessToken,
        cloudfrontUrl: AppGlobal.environment.cloudfrontUrl
      },
      '*'
    );
  }
}
