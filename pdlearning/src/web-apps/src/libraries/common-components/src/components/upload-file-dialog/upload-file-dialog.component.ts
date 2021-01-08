import {
  BaseFormComponent,
  FileUploaderSetting,
  IUploaderProgress,
  LocalTranslatorService,
  ModuleFacadeService,
  UploadProgressStatus
} from '@opal20/infrastructure';
import { Component, Input, ViewChild } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { FileUploaderUtils } from '../../utils/file-uploader.utils';
import { OpalFileUploaderComponent } from '../file-uploader/file-uploader.component';

@Component({
  selector: 'upload-file-dialog',
  templateUrl: './upload-file-dialog.component.html'
})
export class UploadFileDialogComponent extends BaseFormComponent {
  @Input() public canRunValidation: boolean = true;
  public embedHtml: string;
  public get strAllowedExtensions(): string {
    return this.fileUploaderSetting.extensions.join(', ');
  }
  public align: 'left' | 'right' | 'center' = 'left';
  public height: string = 'auto';
  public width: string = 'auto';
  public embedUrl: string;
  public preview: SafeHtml;
  public mode: 'upload' | 'create' = 'create';
  public fileUploaderSetting: FileUploaderSetting;
  public thumbnailUrl: string;
  public confirmCancelDialogRef: DialogRef;

  public uploading: boolean;

  @ViewChild('fileUploader', { static: true })
  private fileUploader: OpalFileUploaderComponent;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private dialogRef: DialogRef,
    public translator: LocalTranslatorService,
    private sanitizer: DomSanitizer
  ) {
    super(moduleFacadeService);
    this.fileUploaderSetting = new FileUploaderSetting();
    this.fileUploaderSetting.extensions = FileUploaderUtils.allowedExtensions;
    this.fileUploaderSetting.isFileLimitSizeValidate = this.canRunValidation;
  }

  public closeDialog(): void {
    if (this.uploading) {
      this.confirmCancelDialogRef = this.modalService.showConfirmMessage(this.translate('Are you sure you want to cancel?'), () => {
        if (this.uploading) {
          this.fileUploader.abortUpload();
          this.dialogRef.close();
        }
      });
      return;
    }
    this.dialogRef.close();
  }

  public onUploaderProgressChanged(uploaderProgressChanged: IUploaderProgress): void {
    switch (uploaderProgressChanged.status) {
      case UploadProgressStatus.Start:
        this.uploading = true;
        break;

      case UploadProgressStatus.Failure:
        this.uploading = false;
        break;

      case UploadProgressStatus.Completed:
        const fileParameter = uploaderProgressChanged.parameters;
        if (this.mode === 'create') {
          this.embedHtml = FileUploaderUtils.wrappingFile(fileParameter, this.width, this.height);
          this.preview = this.sanitizer.bypassSecurityTrustHtml(this.embedHtml);
        }
        break;
    }
  }

  public insertEmbedHtml(): void {
    const el = document.createElement('div');
    el.setAttribute('style', `text-align: ${this.align};`);
    el.innerHTML = this.embedHtml;
    if (Number(this.width) === 0 || Number(this.width) > 99999 || Number(this.height) === 0 || Number(this.height) > 99999) {
      this.confirmCancelDialogRef = this.modalService.showErrorMessage(
        this.translate('Please enter a valid width/height between 1 and 99999')
      );
      this.width = 'auto';
      this.height = 'auto';
      return;
    }
    el.firstElementChild.setAttribute('width', this.width);
    el.firstElementChild.setAttribute('height', this.height);
    this.dialogRef.close(el.outerHTML);
  }
}
