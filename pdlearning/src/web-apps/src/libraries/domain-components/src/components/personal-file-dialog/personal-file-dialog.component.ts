import { BaseComponent, FileUploaderSetting, ModuleFacadeService } from '@opal20/infrastructure';
import { Component, Input, ViewChild } from '@angular/core';

import { BatchUploadFileContentComponent } from '../batch-upload-file-content/batch-upload-file-content.component';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { FileType } from '@opal20/domain-api';
import { PersonalFileListComponent } from '../personal-space-list/personal-file-list.component';
import { UploadStatus } from '../upload-card/upload-card.component';
@Component({
  selector: 'personal-file-dialog',
  templateUrl: './personal-file-dialog.component.html'
})
export class PersonalFileDialogComponent extends BaseComponent {
  public textSearch: string;
  public submitSearch: string = '';
  public isDisplayPersonalFiles: boolean = false;
  public _settings: FileUploaderSetting = new FileUploaderSetting();
  @Input() public icon: string;
  @Input() public uploadFolder: string = 'default';
  @Input() public set settings(v: FileUploaderSetting) {
    this._settings = v;
  }
  @Input() public maxFileCount: number = 20;

  @Input()
  public fileType: FileType[] = [FileType.All];

  @Input()
  public filterByExtensions: string[] = [];

  public get settings(): FileUploaderSetting {
    return this._settings;
  }

  @ViewChild('personalFilesList', { static: false }) public personalFilesList: PersonalFileListComponent;
  @ViewChild('batchUploadFileContent', { static: false }) public batchUploadFileContent: BatchUploadFileContentComponent;

  constructor(public moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef) {
    super(moduleFacadeService);
  }

  public onCancel(): void {
    if (this.batchUploadFileContent.listFile.length + this.personalFilesList.uploadParameters.length > 0) {
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
    if (this.batchUploadFileContent.taskQueue.isRunning) {
      this.modalService.showWarningMessage('Files are uploading, please wait...');
    } else {
      if (this.batchUploadFileContent.cardUploads) {
        if (this.batchUploadFileContent.cardUploads.filter(element => element.status === UploadStatus.Failed).length > 0) {
          this.modalService.showErrorMessage('Some files failed to upload. Please check again.');
        } else {
          const uploadedFiles = this.batchUploadFileContent.cardUploads
            .filter(element => element.status === UploadStatus.Complete)
            .map(element => {
              return element.uploadParameters;
            });
          const selectedPersonalFiles = this.personalFilesList.uploadParameters;
          this.dialogRef.close(uploadedFiles.concat(selectedPersonalFiles));
        }
      } else {
        this.dialogRef.close();
      }
    }
  }

  public onSubmitSearch(): void {
    this.submitSearch = this.textSearch.slice();
  }
}
