import { BaseComponent, FileUploaderSetting, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input, ViewChild } from '@angular/core';

import { BatchUploadFileContentComponent } from '../batch-upload-file-content/batch-upload-file-content.component';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { UploadStatus } from '../upload-card/upload-card.component';

@Component({
  selector: 'batch-upload-files-dialog',
  templateUrl: './batch-upload-files-dialog.component.html'
})
export class BatchUploadFilesDialogComponent extends BaseComponent {
  public get settings(): FileUploaderSetting {
    return this._settings;
  }

  @Input() public icon: string;
  @Input() public uploadFolder: string = 'default';
  @Input() public fileNumber: number = 20;
  @Input() public isPreviewMode: boolean = false;
  @Input() public set settings(v: FileUploaderSetting) {
    this._settings = v;
  }

  @ViewChild('batchUploadFileContent', { static: true }) public batchUploadFileContent: BatchUploadFileContentComponent;
  public _settings: FileUploaderSetting = new FileUploaderSetting();

  constructor(moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef) {
    super(moduleFacadeService);
  }

  public onCancel(): void {
    if (this.batchUploadFileContent.listFile && this.batchUploadFileContent.listFile.length > 0) {
      this.modalService.showConfirmMessage(
        'Do you want to close?',
        () => {
          this.batchUploadFileContent.taskQueue.stop();
          this.dialogRef.close();
        },
        null,
        null
      );
    } else {
      this.dialogRef.close();
    }
  }

  public onSave(): void {
    if (this.batchUploadFileContent.taskQueue && this.batchUploadFileContent.taskQueue.isRunning) {
      this.modalService.showWarningMessage('Files are uploading, please wait...');
    } else {
      if (this.batchUploadFileContent.cardUploads) {
        if (this.batchUploadFileContent.cardUploads.filter(element => element.status === UploadStatus.Failed).length > 0) {
          this.modalService.showErrorMessage('Some files failed to upload. Please check again.');
        } else {
          this.dialogRef.close(
            this.batchUploadFileContent.cardUploads
              .filter(element => element.status === UploadStatus.Complete)
              .map(element => {
                return element.uploadParameters;
              })
          );
        }
      } else {
        this.dialogRef.close();
      }
    }
  }
}
