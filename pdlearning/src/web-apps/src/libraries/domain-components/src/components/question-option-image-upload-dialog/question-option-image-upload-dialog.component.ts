import {
  BaseFormComponent,
  FileUploaderSetting,
  IUploaderProgress,
  ModuleFacadeService,
  UploadParameters,
  UploadProgressStatus
} from '@opal20/infrastructure';
import { Component, ViewChild } from '@angular/core';
import { IOptionMedia, MediaType } from '@opal20/domain-api';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { OpalFileUploaderComponent } from '@opal20/common-components';

@Component({
  selector: 'question-option-image-upload-dialog',
  templateUrl: './question-option-image-upload-dialog.component.html'
})
export class QuestionOptionImageUploadDialogComponent extends BaseFormComponent {
  public thumbnailUrl: string;
  public uploading: boolean;
  public fileUploaderSetting: FileUploaderSetting;
  public confirmCancelDialogRef: DialogRef;
  @ViewChild('fileUploader', { static: true })
  private fileUploader: OpalFileUploaderComponent;

  constructor(protected moduleFacadeService: ModuleFacadeService, private dialogRef: DialogRef) {
    super(moduleFacadeService);
    this.fileUploaderSetting = new FileUploaderSetting();
    this.fileUploaderSetting.isCropImage = false;
    this.fileUploaderSetting.extensions = ['jpeg', 'jpg', 'gif', 'png', 'svg', 'mp4', 'm4v', 'ogv'];
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
        const media = this.setMediaType(uploaderProgressChanged.parameters);
        this.moduleFacadeService.spinnerService.hide();
        this.dialogRef.close(media);
        this.closeConfirmCancelDialog();
        break;
    }
  }

  private setMediaType(fileParameter: UploadParameters): IOptionMedia {
    let mediaType: MediaType;
    switch (fileParameter.fileExtension) {
      case 'mp4':
      case 'm4v':
      case 'ogv':
        mediaType = MediaType.Video;
        break;
      default:
        mediaType = MediaType.Image;
        break;
    }
    const media: IOptionMedia = {
      type: mediaType,
      src: fileParameter.fileLocation
    };

    return media;
  }

  private closeConfirmCancelDialog(): void {
    // if cancel confirm dialog is opening => close it
    if (this.confirmCancelDialogRef) {
      this.confirmCancelDialogRef.close();
    }
  }
}
