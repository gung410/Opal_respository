import { Component, ElementRef, EventEmitter, Input, NgZone, OnChanges, OnDestroy, Output, SimpleChanges, ViewChild } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { Subscription, interval } from 'rxjs';

import { BaseComponent } from '../base-components/base-component';
import { ModuleFacadeService } from '../services/module-facade.service';
import { take } from 'rxjs/operators';

@Component({
  selector: 'document-viewer',
  template: `
    <div class="document-viewer" *ngIf="configuredViewer === 'google' && isIframeUnloaded">
      <div>{{ 'Failed to load the document' | globalTranslator }}</div>
      <button class="k-button reload-btn" (click)="reloadDocument()">{{ 'Reload' | globalTranslator }}</button>
    </div>
    <div class="loading-container" [hidden]="!isShowSpinner">
      <img class="loading-icon" src="assets/images/loading.gif" alt="Loading" />
    </div>
    <div class="iframe-container">
      <div class="cover-btn" [hidden]="(configuredViewer === 'google' && isIframeUnloaded) || !isLoaded"></div>
      <iframe #documentIframe *ngIf="safeDocumentUrl" frameBorder="0" [src]="safeDocumentUrl" (load)="onIframeLoaded($event)"> </iframe>
    </div>
  `,
  styles: [
    `
      :host {
        display: flex;
        flex-direction: column;
      }
      iframe {
        width: 100%;
        height: 100%;
        flex-grow: 1;
      }
      .document-viewer {
        display: flex;
        align-items: center;
        padding: 10px;
        flex-direction: column;
      }
      .iframe-container {
        position: relative;
        width: 100%;
        height: 100%;
      }
      .cover-btn {
        background-color: #d0d0d0;
        height: 44px;
        position: absolute;
        right: 12px;
        top: 11px;
        width: 44px;
      }
      @media (min-width: 769px) and (max-width: 1099px) {
        .cover-btn {
          right: 11px;
          background-image: linear-gradient(90deg, #bbbbbb, #d2cece 10px, #d0d0d0 10%);
        }
      }
      @media (min-width: 376px) and (max-width: 768px) {
        .cover-btn {
          right: 11px;
          background-image: linear-gradient(90deg, #c7c4c4, #d2cece 5px, #d0d0d0 10%);
        }
      }
      @media (max-width: 375px) {
        .cover-btn {
          right: 14px;
        }
      }
      .reload-btn {
        margin-top: 10px;
      }
      .loading-container {
        text-align: center;
      }
      .loading-icon {
        width: 50px;
      }
    `
  ]
})
export class DocumentViewerComponent extends BaseComponent implements OnChanges, OnDestroy {
  public safeDocumentUrl: SafeResourceUrl = null;
  public isLoaded: boolean = false;
  @Output() public loaded: EventEmitter<void> = new EventEmitter();
  @Input() public url = '';
  @Input() public googleCheckInterval = 3500;
  @Input() public showSpinner = false;
  @Input() public set viewer(viewer: string) {
    const v = viewer.toLowerCase().trim();
    if (v !== 'google' && v !== 'office') {
      console.error(`Unsupported viewer: '${viewer}'. Supported viewers: google, office`);
    }
    this.configuredViewer = v;
  }

  @ViewChild('documentIframe', { static: false })
  set documentIframe(elRef: ElementRef) {
    if (!elRef) {
      return;
    }
    this._iframe = elRef;
    this.reloadIframeIfEmpty(elRef.nativeElement);
  }
  get documentIframe(): ElementRef {
    return this._iframe;
  }

  public get isShowSpinner(): boolean {
    return !this.isLoaded && this.showSpinner;
  }
  public isIframeUnloaded: boolean;
  public configuredViewer = 'google';

  private _iframe: ElementRef;
  private checkIFrameSubscription: Subscription = null;
  private documentUrl: string;

  constructor(private domSanitizer: DomSanitizer, private ngZone: NgZone, protected moduleFacadeService: ModuleFacadeService) {
    super(moduleFacadeService);
  }

  public ngOnDestroy(): void {
    if (this.checkIFrameSubscription) {
      this.checkIFrameSubscription.unsubscribe();
    }
  }

  public ngOnChanges(changes: SimpleChanges): void {
    if (
      (changes.url && changes.url.currentValue !== changes.url.previousValue) ||
      (changes.viewer && changes.viewer.currentValue !== changes.viewer.previousValue)
    ) {
      const u = this.url.indexOf('/') ? encodeURIComponent(this.url) : this.url;
      this.documentUrl =
        this.configuredViewer === 'google'
          ? `https://docs.google.com/gview?url=${u}&embedded=true`
          : `https://view.officeapps.live.com/op/embed.aspx?src=${u}`;
      this.safeDocumentUrl = this.domSanitizer.bypassSecurityTrustResourceUrl(this.documentUrl);
      this.moduleFacadeService.globalSpinnerService.show();
    }
  }

  public onIframeLoaded(event: Event): void {
    const iframeTarget = event.target as HTMLFrameElement;
    // For browsers on iOS, when gview 204 it's also go to "loaded".
    // So we have to check the content of iframe to make sure it's loaded or not.
    if (iframeTarget.src !== this.documentUrl || !this._iframe || !!this._iframe.nativeElement.contentDocument) {
      // If we are able to read the iframe content, it's not loaded.
      return;
    }
    this.isIframeUnloaded = false;
    this.moduleFacadeService.globalSpinnerService.hide();
    this.loaded.emit();
    this.isLoaded = true;
    if (this.checkIFrameSubscription) {
      this.checkIFrameSubscription.unsubscribe();
    }
  }

  public reloadDocument(): void {
    if (this.documentIframe && this.documentIframe.nativeElement) {
      this.moduleFacadeService.globalSpinnerService.show();
      this.isIframeUnloaded = undefined;
      this.reloadIFrame(this.documentIframe.nativeElement);

      setTimeout(() => {
        this.moduleFacadeService.globalSpinnerService.hide();
        this.isIframeUnloaded = this.checkIsIframeUnloaded();
      }, this.googleCheckInterval);
    }
  }

  private checkIsIframeUnloaded(): boolean {
    if (this.isIframeUnloaded === false) {
      this.isLoaded = true;
      return this.isIframeUnloaded;
    }
    // If we are able to read the iframe content, it's not loaded.
    this.isLoaded = true;
    return this._iframe && !!this._iframe.nativeElement.contentDocument;
  }

  private reloadIframeIfEmpty(iframe: HTMLIFrameElement): void {
    if (this.configuredViewer !== 'google') {
      return;
    }

    if (this.checkIFrameSubscription !== null) {
      return;
    }

    const spinnerTimeout = 15000;
    const numberOfReloadingTimes = Math.round(
      this.googleCheckInterval === 0 ? 0 : (spinnerTimeout - this.googleCheckInterval) / this.googleCheckInterval
    );

    this.checkIFrameSubscription = interval(this.googleCheckInterval)
      .pipe(take(numberOfReloadingTimes))
      .subscribe(intervalIndex => {
        this.reloadIFrame(iframe);

        if (intervalIndex + 1 === numberOfReloadingTimes) {
          setTimeout(() => {
            this.isIframeUnloaded = this.checkIsIframeUnloaded();
          }, this.googleCheckInterval);
        }
      });
  }

  private reloadIFrame(iframe: HTMLIFrameElement): void {
    this.isLoaded = false;
    if (iframe) {
      iframe.src = iframe.src;
    }
  }
}
