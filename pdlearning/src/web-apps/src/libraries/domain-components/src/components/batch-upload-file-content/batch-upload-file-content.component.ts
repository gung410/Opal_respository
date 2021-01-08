import { BaseComponent, FileUploaderSetting, ModuleFacadeService, UploadParameters } from '@opal20/infrastructure';
import { Component, Input, QueryList, ViewChildren } from '@angular/core';
import { UploadCardComponent, UploadStatus } from '../upload-card/upload-card.component';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { TaskQueue } from './../../helpers/task-queue.helper';

export interface IUploadCardTask {
  task: () => Promise<unknown>;
  id: string;
}

@Component({
  selector: 'batch-upload-file-content',
  templateUrl: './batch-upload-file-content.component.html'
})
export class BatchUploadFileContentComponent extends BaseComponent {
  public get settings(): FileUploaderSetting {
    return this._settings;
  }
  @Input() public maxFileCount: number = 20;
  @Input() public icon: string;
  @Input() public uploadFolder: string = 'default';
  @Input() public isPreviewMode: boolean = false;
  @Input() public set settings(v: FileUploaderSetting) {
    this._settings = v;
  }
  @Input() public selectedFileNumber: number = 0;
  @ViewChildren('uploadCard') public cardUploads: QueryList<UploadCardComponent>;
  public listFile: UploadParameters[] = [];
  public taskQueue = new TaskQueue();
  public _settings: FileUploaderSetting = new FileUploaderSetting();
  constructor(protected moduleFacadeService: ModuleFacadeService, public dialogRef: DialogRef) {
    super(moduleFacadeService);
  }

  public onFileDropped(uploadParametersList: UploadParameters[]): void {
    if (uploadParametersList) {
      if (this.listFile.length + uploadParametersList.length + this.selectedFileNumber > this.maxFileCount) {
        this.modalService.showWarningMessage('Maximum number of files is ' + this.maxFileCount);
      } else {
        this.listFile = this.listFile.concat(uploadParametersList);
      }
    }
  }
  public processUpload(card: UploadCardComponent): void {
    if (!this.taskQueue.isRunning) {
      this.taskQueue.clear();
    }
    if (card.status === UploadStatus.New) {
      this.taskQueue.enqueue(card.uploadTask);
    }
    if (!this.taskQueue.isRunning) {
      this.taskQueue.start();
    }
  }

  public triggerUpload(card: UploadCardComponent): void {
    this.processUpload(card);
  }

  public deleteFile(card: UploadCardComponent): void {
    this.taskQueue.dequeue(card.uploadTask);
    card.abortUpload();
    const cardIndex = this.listFile.indexOf(card.uploadParameters);
    this.listFile.splice(cardIndex, 1);
    if (this.listFile.filter(element => element.isProcessing).length === 0) {
      this.taskQueue.start();
    }
  }
}
