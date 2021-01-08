import {
  AmazonS3UploaderService,
  BaseComponent,
  FileUploaderSetting,
  IMultipartFileInfo,
  ModuleFacadeService,
  UploadParameters
} from '@opal20/infrastructure';
import { Component, ElementRef, ViewChild } from '@angular/core';

import { CropImageDialogComponent } from '../crop-image-dialog/crop-image-dialog.component';
import { DialogRef } from '@progress/kendo-angular-dialog';
import { FileUploaderUtils } from '../../utils/file-uploader.utils';
import { UploadProgressOptions } from '../../models/upload-progress-options';

@Component({
  selector: 'upload-image-dialog',
  templateUrl: './upload-image-dialog.component.html',
  styleUrls: ['./upload-image-dialog.component.scss']
})
export class UploadImageDialogComponent extends BaseComponent {
  public parameters: UploadParameters | undefined;
  public percentage: number = 0;
  public thumbnailUrl: string | undefined;
  public fileUploaderSetting: FileUploaderSetting;
  private progressContext: CanvasRenderingContext2D;
  private progressOptions: UploadProgressOptions = new UploadProgressOptions(2, 160);

  @ViewChild('progressCanvas', { static: false })
  private progressCanvas: ElementRef;

  @ViewChild('inputRef', { static: false })
  private inputElementRef: ElementRef;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    protected dialogRef: DialogRef,
    private uploaderService: AmazonS3UploaderService
  ) {
    super(moduleFacadeService);
    this.fileUploaderSetting = new FileUploaderSetting();
    this.fileUploaderSetting.extensions = ['jpeg', 'jpg', 'gif', 'png', 'svg'];
  }

  public showCropImageDialog(file: File): void {
    const dialogRef: DialogRef = this.moduleFacadeService.dialogService.open({ content: CropImageDialogComponent });
    const component: CropImageDialogComponent = dialogRef.content.instance;
    component.file = file;
    dialogRef.result.toPromise().then((cropedfile: File) => {
      if (cropedfile && cropedfile instanceof Blob) {
        this.onFileSelectToUpload(cropedfile);
      }
    });
  }

  public onFileSelect(files: FileList): void {
    const file: File = files.item(0);
    if (!file) {
      return;
    }
    this.uploaderService
      .validateMineType(file, this.fileUploaderSetting)
      .then((info: IMultipartFileInfo) => {
        if (FileUploaderUtils.canCropImage(info.extension, this.fileUploaderSetting.isCropImage)) {
          this.showCropImageDialog(file);
        }
        this.onFileSelectToUpload(file);
      })
      .catch(error => {
        this.moduleFacadeService.modalService.showErrorMessage(error.message || error);
      });
  }

  public addImage(): void {
    this.dialogRef.close(this.thumbnailUrl);
  }

  public performCancel(): void {
    this.dialogRef.close();
  }

  private onFileSelectToUpload(file: File): void {
    this.thumbnailUrl = undefined;

    this.uploaderService
      .validateMineType(file, this.fileUploaderSetting)
      .then((info: IMultipartFileInfo) => {
        this.parameters = new UploadParameters();
        this.parameters.file = file;
        this.parameters.folder = 'editor-images';
        this.parameters.fileExtension = info.extension;
        this.parameters.mineType = info.mimeType;
        this.parameters.onUpdateProgress = (percentage: number) => {
          this.drawScene(percentage);
        };
        this.createSurface();

        return this.uploaderService
          .uploadFile(this.parameters)
          .then(() => (this.thumbnailUrl = `${AppGlobal.environment.cloudfrontUrl}/${this.parameters.fileLocation}`));
      })
      .catch(error => {
        this.moduleFacadeService.modalService.showErrorMessage(error.message || error);
        this.inputElementRef.nativeElement.value = '';
      });
  }

  private createSurface(): void {
    this.progressContext = (<HTMLCanvasElement>this.progressCanvas.nativeElement).getContext('2d');
    this.progressContext.translate(0.5, 0.5);
  }

  private drawScene(percentage: number): void {
    const deegres: number = percentage * (360 / 100);
    const radius: number = this.progressOptions.canvasWidth / 2 - this.progressOptions.lineWidth * 2;
    this.progressContext.clearRect(0, 0, this.progressOptions.canvasWidth, this.progressOptions.canvasWidth);

    this.progressContext.beginPath();
    this.progressContext.arc(
      this.progressOptions.posX,
      this.progressOptions.posY,
      radius,
      (Math.PI / 180) * 270,
      (Math.PI / 180) * (270 + 360)
    );
    this.progressContext.strokeStyle = '#DFE0DF';
    this.progressContext.lineWidth = this.progressOptions.lineWidth;
    this.progressContext.stroke();

    this.progressContext.beginPath();
    this.progressContext.strokeStyle = '#83ACE3';
    this.progressContext.lineWidth = this.progressOptions.lineWidth;
    this.progressContext.arc(
      this.progressOptions.posX,
      this.progressOptions.posY,
      radius,
      (Math.PI / 180) * 270,
      (Math.PI / 180) * (270 + deegres)
    );
    this.progressContext.stroke();

    this.percentage = percentage;
  }
}
