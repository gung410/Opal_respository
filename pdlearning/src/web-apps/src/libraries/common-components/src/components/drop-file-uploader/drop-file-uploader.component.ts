import {
  AmazonS3UploaderService,
  BaseComponent,
  FileUploaderSetting,
  IMultipartFileInfo,
  ModuleFacadeService,
  UploadParameters
} from '@opal20/infrastructure';
import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';

import { FileUploaderUtils } from './../../utils/file-uploader.utils';
import { OpalDialogService } from './../../services/dialog/dialog.service';

@Component({
  selector: 'drop-file-uploader',
  templateUrl: './drop-file-uploader.component.html'
})
export class DropFileUploaderComponent extends BaseComponent {
  public uploadParameters: UploadParameters;
  public parameters: UploadParameters | undefined;
  public percentage: number = 0;
  public thumbnailUrl: string | undefined;
  public isFileDropOver: boolean = false;
  public height: string = 'auto';
  public width: string = 'auto';

  @Input() public set settings(v: FileUploaderSetting) {
    this._settings = v;
    if (!v.extensions) {
      this._settings.extensions = FileUploaderUtils.allowedExtensions;
    }
  }
  public get settings(): FileUploaderSetting {
    return this._settings;
  }
  @Input() public uploadFolder = 'editor-images';
  @Input() public onValidatingFile: boolean = false;

  @Output() public onFileDropped = new EventEmitter<String>();
  @Output() public onTextDropped = new EventEmitter<String>();
  @Output() public onFileDragLeave = new EventEmitter<void>();
  @Output() public onFileDragOver = new EventEmitter<void>();

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

  @HostListener('body:dragover', ['$event'])
  public onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.onFileDragOver.emit();
  }

  @HostListener('body:dragleave', ['$event'])
  public onDragLeave(evt: DragEvent): void {
    evt.preventDefault();
    evt.stopPropagation();

    // Check when user drag leave outside body
    if (!evt.clientX && !evt.clientY) {
      this.onFileDragLeave.emit();
    }
  }

  @HostListener('drop', ['$event'])
  public onDrop(evt: DragEvent): void {
    evt.preventDefault();
    evt.stopPropagation();
    const file = evt.dataTransfer.files[0];
    const textDrag = evt.dataTransfer.getData('Text');

    if (evt.dataTransfer.files.length > 1) {
      this.modalService.showWarningMessage('Drag and drop 1 file at a time.');
      this.onFileDropped.emit();
      return;
    }

    if (textDrag) {
      this.onTextDropped.emit(textDrag);
      return;
    }

    // Case drop a folder into drop-zone
    if (file.size === 0) {
      this.onFileDropped.emit();
      return;
    }

    if (file) {
      this.processUploadFile(file).then(() => {
        this.onValidatingFile = false;
      });
    } else {
      this.onFileDropped.emit();
    }
  }

  // Hide dropzone in case that user do not drop into this dropzone component
  @HostListener('body:drop', ['$event'])
  public onBodyDrop(evt: DragEvent): void {
    this.onFileDropped.emit();
  }

  private processUploadFile(file: File): Promise<void> {
    this.uploadParameters = new UploadParameters();
    this.onValidatingFile = true;
    this.uploadParameters.file = file;

    return Promise.resolve(this.uploadParameters)
      .then(() => this.validateFile(this.uploadParameters))
      .then(() => this.processCropFile(this.uploadParameters))
      .then(() => this.uploadFile(this.uploadParameters))
      .then(() => this.parseHtmlForEditor(this.uploadParameters))
      .catch(err => {
        this.modalService.showErrorMessage(err);
        this.onFileDropped.emit();
      });
  }

  private validateFile(parameter: UploadParameters): Promise<void> {
    return this.uploaderService.validateMineType(parameter.file, this.settings).then((info: IMultipartFileInfo) => {
      parameter.folder = this.uploadFolder;
      parameter.fileExtension = info.extension;
      parameter.mineType = info.mimeType;
    });
  }

  private processCropFile(parameter: UploadParameters): Promise<void | File> {
    if (FileUploaderUtils.canCropImage(parameter.fileExtension, this.settings.isCropImage)) {
      const cropImageCallback = (cropedFile: File): Promise<void | File> => {
        return new Promise(resolve => {
          parameter.file = undefined;
          if (cropedFile && cropedFile instanceof Blob) {
            parameter.file = cropedFile;
          }
          resolve();
        });
      };

      return FileUploaderUtils.showCropImageDialog(parameter.file, this.opalDialogSvc, cropImageCallback);
    }

    return Promise.resolve();
  }

  private uploadFile(parameter: UploadParameters): Promise<void> {
    if (parameter.file) {
      return this.uploaderService.uploadFile(parameter, false);
    }
    return Promise.resolve();
  }

  private parseHtmlForEditor(parameter: UploadParameters): Promise<void> {
    return new Promise(resolve => {
      if (parameter.file) {
        const src = FileUploaderUtils.wrappingFile(parameter, this.width, this.height);
        this.onFileDropped.emit(src);
      }
      this.onFileDropped.emit();
      resolve();
    });
  }
}
