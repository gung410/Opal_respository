import { BaseComponent, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, HostListener, Input, ViewEncapsulation } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

import { EncryptUrlPathHelper } from '../../helpers/encrypt-url-path.helper';

@Component({
  selector: 'opal-report',
  templateUrl: './opal-report.component.html',
  encapsulation: ViewEncapsulation.None
})
export class OpalReportComponent extends BaseComponent {
  public safeUrl: SafeResourceUrl = '';
  public height: string = 'auto';

  private _formId: string;
  public get formId(): string {
    return this._formId;
  }
  @Input()
  public set formId(v: string) {
    this._formId = v;
    this.updateUrl();
  }
  private _courseId: string | null;
  public get courseId(): string | null {
    return this._courseId;
  }
  @Input()
  public set courseId(v: string | null) {
    this._courseId = v;
    this.updateUrl();
  }
  private _classrunId: string | null;
  public get classrunId(): string | null {
    return this._classrunId;
  }
  @Input()
  public set classrunId(v: string | null) {
    this._classrunId = v;
    this.updateUrl();
  }

  constructor(protected moduleFacadeService: ModuleFacadeService, private domSanitizer: DomSanitizer) {
    super(moduleFacadeService);
  }

  @HostListener('window:message', ['$event'])
  public onMessage(event: MessageEvent): void {
    this.receiveMessageFromIframe(event);
  }

  private updateUrl(): void {
    const urlString = AppGlobal.environment.reportUrl + '?q=';
    let queryStringFormId = 'statistics/forms/?formId=' + this.formId;

    if (this.courseId) {
      queryStringFormId += '&courseId=' + this.courseId;
    }
    if (this.classrunId) {
      queryStringFormId += '&classrunId=' + this.classrunId;
    }

    // Encrypt AES querystring url
    const encrypted = EncryptUrlPathHelper.encrypt(queryStringFormId);

    this.safeUrl = this.domSanitizer.bypassSecurityTrustResourceUrl(urlString + encrypted);
  }

  private receiveMessageFromIframe(event: MessageEvent): void {
    const data = event.data.params;

    if (!data) {
      return;
    }

    if (!data.height) {
      return;
    }

    this.height = data.height;
  }
}
