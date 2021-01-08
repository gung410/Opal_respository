import { AfterViewInit, Component, HostListener, forwardRef } from '@angular/core';
import {
  AmazonS3UploaderService,
  BaseFormComponent,
  LocalTranslatorService,
  ModuleFacadeService,
  UploadParameters
} from '@opal20/infrastructure';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
@Component({
  selector: 'math-live-editor-dialog',
  templateUrl: './math-live-editor-dialog.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => MathLiveEditorDialogComponent),
      multi: true
    }
  ]
})
export class MathLiveEditorDialogComponent extends BaseFormComponent implements AfterViewInit {
  public svg: string;
  public safeUrl: SafeResourceUrl;
  public embedHtml: string;
  public embedUrl: string;
  public isSubmiting: boolean = false;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private dialogRef: DialogRef,
    public translator: LocalTranslatorService,
    private uploaderService: AmazonS3UploaderService,
    private sanitizer: DomSanitizer
  ) {
    super(moduleFacadeService);
    this.safeUrl = sanitizer.bypassSecurityTrustResourceUrl('mathlive/index.html');
  }

  @HostListener('window:message', ['$event'])
  public onMessage(event: MessageEvent): void {
    try {
      const messageData = event.data;
      if (messageData.key === 'MathliveContentChanged') {
        this.svg = messageData.content;
      }
    } catch {
      return;
    }
  }

  public onLoad(): void {
    const iframe: HTMLIFrameElement = document.getElementById('mathLiveIframe') as HTMLIFrameElement;
    iframe.contentWindow.parent.postMessage({ key: 'MathliveLoaded' }, '*');
    iframe.contentWindow.postMessage({ key: 'MathliveOpen', content: '123' }, '*');
  }

  public closeDialog(): void {
    this.dialogRef.close();
  }

  public insertEmbedHtml(): void {
    const svgParameters = new ISvgParameters();
    this.isSubmiting = true;

    Promise.resolve(svgParameters)
      .then(() => this.getSvgHtmlConent(svgParameters))
      .then(() => this.getLatexCodeFromSvgHtml(svgParameters))
      .then(() => this.createUploadParamFromSvgHtml(svgParameters))
      .then(() => this.uploadFile(svgParameters))
      .catch(err => {
        this.moduleFacadeService.modalService.showErrorMessage('Something wrong, please try againt!', () => {
          this.dialogRef.close();
        });
      })
      .then(() => {
        this.isSubmiting = false;
        this.dialogRef.close(svgParameters);
      });
  }

  public canShowSubmitBtn(): boolean {
    // if nothing input, the html of svg has min length = 382
    return !this.svg || this.svg.length === 382;
  }

  private async getSvgHtmlConent(parameters: ISvgParameters): Promise<void> {
    return new Promise(resolve => {
      parameters.svgContent = this.svg;
      resolve();
    });
  }

  private async createUploadParamFromSvgHtml(parameters: ISvgParameters): Promise<void> {
    return new Promise(resolve => {
      const blob = new Blob([parameters.svgContent], { type: 'image/svg+xml' });
      parameters.uploadParameters.file = new File([blob], 'mathsymbol-svg', { lastModified: Date.now() });
      parameters.uploadParameters.fileExtension = 'svg';
      parameters.uploadParameters.mineType = 'image/svg+xml';
      parameters.uploadParameters.folder = 'editor-images';
      resolve();
    });
  }

  private async uploadFile(parameters: ISvgParameters): Promise<void> {
    return new Promise(async resolve => {
      await this.uploaderService.uploadFile(parameters.uploadParameters, false);
      parameters.s3Url = `${AppGlobal.environment.cloudfrontUrl}/${parameters.uploadParameters.fileLocation}`;
      resolve();
    });
  }

  private async getLatexCodeFromSvgHtml(parameters: ISvgParameters): Promise<void> {
    return new Promise(resolve => {
      const latexPattern = /\s(latexcode)="(?<code>.*?)"/gim;
      const latexCodeMatches = latexPattern.exec(parameters.svgContent);
      // tslint:disable-next-line:no-string-literal
      parameters.latexCode = latexCodeMatches.groups['code'];
      resolve();
    });
  }
}

export class ISvgParameters {
  constructor(
    public svgContent?: string,
    public latexCode?: string,
    public s3Url?: string,
    public uploadParameters: UploadParameters = new UploadParameters()
  ) {}
}
