import {
  AmazonS3UploaderService,
  BaseComponent,
  FileUploaderSetting,
  IMultipartFileInfo,
  IUploaderProgress,
  ModuleFacadeService,
  NotificationType,
  UploadParameters,
  UploadProgressStatus
} from '@opal20/infrastructure';
import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  Output,
  ViewChild,
  ViewEncapsulation,
  forwardRef
} from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

import { FileUploaderUtils } from '../../utils/file-uploader.utils';
import { OpalDialogService } from '../../services/dialog/dialog.service';
import { UploadProgressOptions } from '../../models/upload-progress-options';

export type AdditionalValidationFn = (info: IMultipartFileInfo) => Promise<IMultipartFileInfo>;

@Component({
  selector: 'opal-file-uploader',
  templateUrl: './file-uploader.component.html',
  encapsulation: ViewEncapsulation.None,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OpalFileUploaderComponent),
      multi: true
    }
  ]
})
export class OpalFileUploaderComponent extends BaseComponent implements ControlValueAccessor {
  public static graphicsExtensions: string[] = ['jpeg', 'jpg', 'gif', 'png', 'svg'];

  public onChange?: (_: string) => void;
  public onTouched?: (_: unknown) => void;
  public value: string | null;

  @Input() public uploadFolder: string = 'default';
  @Input() public readOnly: boolean = false;

  @Input() public set settings(v: FileUploaderSetting) {
    this._settings = v;
    if (!v.extensions) {
      this._settings.extensions = FileUploaderUtils.allowedExtensions;
    }
  }
  public get settings(): FileUploaderSetting {
    return this._settings;
  }
  @Input() public additionalValidationFn: AdditionalValidationFn;
  @Input() public showBtnDelete: boolean = false;
  /**
   * Set `keepInlocal` true to keep the file in local.
   * Then trigger `triggerUploadingFile` function to push the file to cloud.
   */
  @Input() public keepInLocal: boolean = false;
  @Output() public uploaderProgressChange: EventEmitter<IUploaderProgress> = new EventEmitter();
  @Input() public showAdditionalProcessStatus?: boolean = false;

  public uploadingFile: boolean = false;
  public validatingFile: boolean = false;
  public uploadingPercentage: number = 0;

  public isError: boolean = false;
  public errorMessage: string;

  public file: File;

  @ViewChild('progressCanvas', { static: true }) private progressCanvas: ElementRef;
  @ViewChild('inputRef', { static: true }) private inputElementRef: ElementRef;
  private progressContext: CanvasRenderingContext2D;
  private progressOptions: UploadProgressOptions = new UploadProgressOptions(2, 160);
  private uploadParameters: UploadParameters;
  private _settings: FileUploaderSetting = new FileUploaderSetting();

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    public changeDetectorRef: ChangeDetectorRef,
    public elementRef: ElementRef,
    private opalDialogSvc: OpalDialogService,
    private uploaderService: AmazonS3UploaderService
  ) {
    super(moduleFacadeService);
    this.additionalValidationFn = (info: IMultipartFileInfo) => Promise.resolve(info);
  }
  public onInit(): void {
    this.createSurface();
    this.drawScene(0);
  }
  public writeValue(value?: string): void {
    this.value = value;
  }
  public registerOnChange(fn: (_: unknown) => void): void {
    this.onChange = fn;
  }
  public registerOnTouched(fn: (_: unknown) => void): void {
    this.onTouched = fn;
  }

  public onFileSelect(files: FileList): void {
    if (files.length > 0) {
      const file = files.item(0);
      this.validatingFile = true;
      this.uploaderService
        .validateMineType(file, this.settings)
        .then((info: IMultipartFileInfo) => {
          if (FileUploaderUtils.canCropImage(info.extension, this.settings.isCropImage)) {
            return this.showCropImageDialog(file);
          }
          this.file = file;
          if (this.keepInLocal === false) {
            this.uploadFile(this.file);
          }
        })
        .catch(error => {
          this.inputElementRef.nativeElement.value = '';
          this.showError(error ? error.message || error : 'Upload Failed');
        })
        .finally(() => {
          this.validatingFile = false;
        });
    }
  }

  public triggerUploadingFile(): Promise<string | void> {
    if (!this.file) {
      return Promise.reject('File not found');
    }
    return this.uploadFile(this.file);
  }

  public showCropImageDialog(file: File): Promise<string | void> {
    const cropImageCallback = (cropedFile: File): Promise<void> => {
      return new Promise(resolve => {
        if (cropedFile && cropedFile instanceof Blob) {
          this.file = new File([cropedFile], file.name);
        }
        resolve();
      });
    };

    return FileUploaderUtils.showCropImageDialog(file, this.opalDialogSvc, cropImageCallback).then(() => {
      if (this.file && this.keepInLocal === false) {
        return this.uploadFile(this.file, this.settings.isFileLimitSizeValidate);
      } else {
        this.inputElementRef.nativeElement.value = '';
        return Promise.resolve();
      }
    });
  }

  public abortUpload(): void {
    if (this.uploadParameters && this.uploadingFile) {
      this.uploaderService
        .abortUpload({
          fileExtension: this.uploadParameters.fileExtension,
          folder: this.uploadParameters.folder,
          fileId: this.uploadParameters.fileId,
          uploadId: this.uploadParameters.uploadId
        })
        .then(() => {
          this.inputElementRef.nativeElement.value = '';
          this.file = undefined;
          this.uploadParameters = undefined;
          this.drawScene(0);
        });
    }
  }

  public onTryAgain(): void {
    this.resetUpload();
    this.inputElementRef.nativeElement.click();
  }

  public resetUpload(): void {
    this.isError = false;
    this.value = null;
    this.updateValue(null);
    this.inputElementRef.nativeElement.value = '';
    this.file = undefined;
    this.uploadingFile = false;
    this.errorMessage = null;
  }

  public showError(errorMessage: string): void {
    this.errorMessage = errorMessage;
    this.isError = true;
  }

  private uploadFile(file: File, canRunValidation: boolean = true): Promise<string | void> {
    this.uploadParameters = new UploadParameters();
    this.uploaderProgressChange.emit({ status: UploadProgressStatus.Start });
    this.validatingFile = true;
    this.settings.isFileLimitSizeValidate = canRunValidation;
    return this.uploaderService
      .validateMineType(file, this.settings)
      .finally(() => {
        this.validatingFile = false;
      })
      .then((info: IMultipartFileInfo) => this.additionalValidationFn(info))
      .then((info: IMultipartFileInfo) => {
        this.uploadParameters.file = file;
        this.uploadParameters.folder = this.uploadFolder;
        this.uploadParameters.fileExtension = info.extension;
        this.uploadParameters.mineType = info.mimeType;
        this.uploadParameters.onUpdateProgress = (percentage: number) => {
          this.drawScene(percentage);

          // Call spinnerService.show() to prevent loading get OFF when it reached to maximum time
          if (percentage < 100) {
            this.moduleFacadeService.spinnerService.show();
          }
        };
        this.createSurface();
        this.uploadingFile = true;
        this.moduleFacadeService.spinnerService.show();

        return this.uploaderService
          .uploadFile(this.uploadParameters, false)
          .then(() => {
            if (OpalFileUploaderComponent.graphicsExtensions.find(ext => ext === this.uploadParameters.fileExtension)) {
              this.updateValue(`${AppGlobal.environment.cloudfrontUrl}/${this.uploadParameters.fileLocation}`);
            }
            this.uploadingFile = false;
            this.moduleFacadeService.spinnerService.hide();
            this.inputElementRef.nativeElement.value = '';
            this.file = undefined;
            this.uploaderProgressChange.emit({ status: UploadProgressStatus.Completed, parameters: this.uploadParameters });
          })
          .then(() => {
            this.showNotification(this.translate('The file is successfully uploaded'), NotificationType.Success);
            return this.uploadParameters.fileLocation
              ? `${AppGlobal.environment.cloudfrontUrl}/${this.uploadParameters.fileLocation}`
              : undefined;
          });
      })
      .catch(error => {
        this.inputElementRef.nativeElement.value = '';
        this.file = undefined;
        this.uploadingFile = false;
        this.uploaderProgressChange.emit({ status: UploadProgressStatus.Failure });
        this.moduleFacadeService.spinnerService.hide();

        this.showError(error ? error.message || error : 'Upload Failed');
      });
  }

  private updateValue(value: string | null): void {
    this.writeValue(value);
    if (this.onChange != null) {
      this.onChange(value);
    }
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

    this.uploadingPercentage = percentage;
  }

  private createSurface(): void {
    this.progressContext = (<HTMLCanvasElement>this.progressCanvas.nativeElement).getContext('2d');
    this.progressContext.translate(0.5, 0.5);
  }
}
