import {
  AmazonS3UploaderService,
  BaseComponent,
  FileUploaderSetting,
  IMultipartFileInfo,
  ModuleFacadeService,
  UploadParameters
} from '@opal20/infrastructure';
import { Component, ElementRef, EventEmitter, HostListener, Input, Output, ViewChild } from '@angular/core';
import { FileUploaderUtils, OpalDialogService } from '@opal20/common-components';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { PackageConfirmDialog } from '../learning-package-confirmation-dialog/learning-package-confirmation-dialog.component';

@Component({
  selector: 'batch-file-uploader',
  templateUrl: './batch-file-uploader.component.html'
})
export class BatchFileUploaderComponent extends BaseComponent {
  public get settings(): FileUploaderSetting {
    return this._settings;
  }
  @Input() public icon: string;
  @Input() public set settings(v: FileUploaderSetting) {
    this._settings = v;
    if (!v.extensions) {
      this._settings.extensions = FileUploaderUtils.allowedExtensions;
    }
  }
  @Input() public totalFile: number;
  @Input() public selectedFileNumber: number = 0;
  @Input() public maxFileCount: number = 20;
  @Output() public onFileDropped = new EventEmitter<UploadParameters[]>();
  @ViewChild('fileInput', { static: false })
  public fileInput: ElementRef;
  public confirmCancelDialogRef: DialogRef;

  private _settings: FileUploaderSetting = new FileUploaderSetting();

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private opalDialogSvc: OpalDialogService,
    private uploaderService: AmazonS3UploaderService
  ) {
    super(moduleFacadeService);
    if (!this.settings.extensions) {
      this.settings.extensions = FileUploaderUtils.allowedExtensions;
    }
  }

  @HostListener('drop', ['$event'])
  public async onDrop(evt: DragEvent): Promise<void> {
    evt.preventDefault();
    evt.stopPropagation();
    this.processFileSelect(evt.dataTransfer.files);
  }

  @HostListener('body:drop', ['$event'])
  public onBodyDrop(evt: DragEvent): void {
    evt.preventDefault();
    evt.stopPropagation();
  }

  public onFileSelect(files: FileList): void {
    this.processFileSelect(files);
    this.fileInput.nativeElement.value = null;
  }

  public additionalValidationFn(): (i: IMultipartFileInfo, fileName: string) => Promise<IMultipartFileInfo> {
    return (info: IMultipartFileInfo, fileName: string) =>
      Promise.resolve().then(
        () =>
          new Promise((resolve: (info: IMultipartFileInfo) => void, reject: (msg?: string) => void) => {
            if (info.extension === 'zip') {
              const chooseTypeDialog: DialogRef = this.opalDialogSvc.openDialogRef(PackageConfirmDialog);
              chooseTypeDialog.content.instance.fileName = fileName;
              return chooseTypeDialog.result.toPromise().then((result: IDataItem) => {
                if (result.hasOwnProperty('value')) {
                  info.extension = result.value as string;
                  info.mimeType = `application/${result.value}`;
                  return resolve(info);
                }
                return reject();
              });
            }
            return resolve(info);
          })
      );
  }

  private processFileSelect(files: FileList): Promise<void> {
    if (files && files.length > 0) {
      if (files.length > this.maxFileCount || this.selectedFileNumber + files.length + this.totalFile > this.maxFileCount) {
        this.modalService.showWarningMessage('Maximum number of files is ' + this.maxFileCount);
        return;
      }
      return this.validateFiles(files)
        .then((uploadParameters: UploadParameters[]) => this.processFiles(uploadParameters))
        .then((uploadParameters: UploadParameters[]) => {
          uploadParameters = uploadParameters.filter(param => param.file);
          this.onFileDropped.emit(uploadParameters);
        })
        .catch(error => {
          this.modalService.showErrorMessage(error);
        });
    }
  }

  private validateFiles(files: FileList): Promise<UploadParameters[]> {
    return Promise.all(
      Array.from(files).map(element => {
        if (element) {
          return this.validateFile(element);
        }
      })
    );
  }

  private validateFile(file: File): Promise<UploadParameters> {
    return this.uploaderService
      .validateMineType(file, this.settings)
      .then((info: IMultipartFileInfo) => this.additionalValidationFn()(info, file.name))
      .then((info: IMultipartFileInfo) => {
        const uploadParameters = new UploadParameters();
        uploadParameters.file = file;
        uploadParameters.fileName = file.name;
        uploadParameters.fileSize = file.size;
        uploadParameters.fileExtension = info.extension;
        uploadParameters.mineType = info.mimeType;
        return uploadParameters;
      });
  }

  private processFiles(validParameters: UploadParameters[]): Promise<UploadParameters[]> {
    return Array.from(validParameters)
      .reduce((previousPromise, element) => {
        return previousPromise.then(() => {
          if (element) {
            return this.processCropFile(element).then((croppedFile: File) => {
              element.file = croppedFile;
            });
          }
        });
      }, Promise.resolve())
      .then(() => {
        return validParameters;
      });
  }

  private processCropFile(uploadParameters: UploadParameters): Promise<void | File> {
    const fileExtension = uploadParameters.file.name.substr(uploadParameters.file.name.lastIndexOf('.') + 1);
    if (FileUploaderUtils.canCropImage(fileExtension, this.settings.isCropImage)) {
      return this.cropFile(uploadParameters.file);
    } else {
      return Promise.resolve(uploadParameters.file);
    }
  }

  private cropFile(file: File): Promise<void | File> {
    const cropImageCallback = (cropedFile: File): Promise<void | File> => {
      return new Promise(resolve => {
        if (cropedFile && cropedFile instanceof Blob) {
          const croppedfile = new File([cropedFile], file.name, {
            type: file.type
          });
          resolve(croppedfile);
        }
        resolve();
      });
    };
    return FileUploaderUtils.showCropImageDialog(file, this.opalDialogSvc, cropImageCallback);
  }
}
