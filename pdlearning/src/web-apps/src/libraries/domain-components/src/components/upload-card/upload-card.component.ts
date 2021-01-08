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
import { ChangeDetectorRef, Component, ElementRef, EventEmitter, Input, Output, ViewEncapsulation, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

import { Task } from '../../helpers/task-queue.helper';

export type AdditionalValidationFn = (info: IMultipartFileInfo) => Promise<IMultipartFileInfo>;

export enum UploadStatus {
  New = 'New',
  Validating = 'Validating',
  ProcessingScorm = 'Processing Scorm Package',
  Uploading = 'Uploading',
  Complete = 'Completed',
  Failed = 'Failed'
}

@Component({
  selector: 'upload-card',
  templateUrl: './upload-card.component.html',
  encapsulation: ViewEncapsulation.None,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => UploadCardComponent),
      multi: true
    }
  ]
})
export class UploadCardComponent extends BaseComponent implements ControlValueAccessor {
  public static graphicsExtensions: string[] = ['jpeg', 'jpg', 'gif', 'png', 'svg'];

  public onChange?: (_: string) => void;
  public onTouched?: (_: unknown) => void;

  public value: string | null;
  public uploadingPercentage: number = 0;
  public status: string = UploadStatus.New;
  public errorMessage: string;
  @Input() public uploadParameters: UploadParameters;
  @Input() public uploadFolder: string = 'default';
  @Input() public isTemporary: boolean = false;
  @Input() public set settings(v: FileUploaderSetting) {
    this._settings = v;
  }
  public get settings(): FileUploaderSetting {
    return this._settings;
  }
  @Input() public additionalValidationFn: AdditionalValidationFn;

  /**
   * Set `keepInlocal` true to keep the file in local.
   * Then trigger `triggerUploadingFile` function to push the file to cloud.
   */
  @Input() public keepInLocal: boolean = true;
  @Output() public uploaderProgressChange: EventEmitter<IUploaderProgress> = new EventEmitter();
  @Output() public triggerUpload = new EventEmitter();
  @Output() public deleteFile = new EventEmitter();
  private _settings: FileUploaderSetting = new FileUploaderSetting();

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    public changeDetectorRef: ChangeDetectorRef,
    public elementRef: ElementRef,
    private uploaderService: AmazonS3UploaderService
  ) {
    super(moduleFacadeService);
    this.additionalValidationFn = (info: IMultipartFileInfo) => Promise.resolve(info);
  }

  public onInit(): void {
    this.status = UploadStatus.New;
    this.uploadParameters.folder = this.uploadFolder;
    this.uploadParameters.isTemporary = this.isTemporary;
    this.uploadParameters.onChangeStatus = (status, error?) => {
      if (error) {
        this.status = UploadStatus.Failed;
        this.errorMessage = error;
      } else {
        this.status = status;
        this.errorMessage = undefined;
      }
    };
    this.uploadParameters.onUpdateProgress = (percentage: number) => {
      this.uploadingPercentage = percentage;
    };
  }

  public ngAfterContentInit(): void {
    this.triggerUpload.emit(this);
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

  public uploadTask: Task<unknown> = () => Promise.resolve(this.uploadEventHandler());

  public onDelete(): void {
    this.deleteFile.emit(this);
  }

  public triggerUploadingFile(file: File): Promise<string | void> {
    if (!file) {
      return Promise.reject('File not found');
    }

    return Promise.resolve(this.uploadParameters)
      .then(() => this.uploadFile(this.uploadParameters))
      .catch(error => {
        let errorMessage: string;
        if (typeof error === 'string') {
          errorMessage = error;
        } else {
          errorMessage = error.message;
        }
        this.status = UploadStatus.Failed;
        this.errorMessage = errorMessage;
        this.uploaderProgressChange.emit({ status: UploadProgressStatus.Failure });
      });
  }

  public abortUpload(): void {
    if (this.uploadParameters) {
      this.uploaderService
        .abortUpload({
          fileExtension: this.uploadParameters.fileExtension,
          folder: this.uploadParameters.folder,
          fileId: this.uploadParameters.fileId,
          uploadId: this.uploadParameters.uploadId
        })
        .then(() => {
          this.uploadParameters = undefined;
        });
    }
  }

  public onTryAgain(): void {
    this.resetUpload();
  }

  public resetUpload(): void {
    this.value = null;
    this.status = UploadStatus.New;
    this.errorMessage = null;
  }

  public uploadEventHandler(): Promise<string | void> {
    if (this.status === UploadStatus.New) {
      this.uploadParameters.isProcessing = true;
      return this.triggerUploadingFile(this.uploadParameters.file).then(() => {
        this.uploadParameters.isProcessing = false;
      });
    }
  }

  public showError(errorMessage: string): void {
    this.modalService.showErrorMessage(errorMessage);
    this.errorMessage = errorMessage;
  }

  private uploadFile(uploadParameters: UploadParameters): Promise<string | void> {
    this.uploaderProgressChange.emit({ status: UploadProgressStatus.Start });
    this.status = UploadStatus.Uploading;
    return Promise.resolve(uploadParameters)
      .then(() => {
        return uploadParameters.isPersonalFile === true
          ? this.uploaderService.uploadPersonalFile(uploadParameters, false)
          : this.uploaderService.uploadFile(uploadParameters, false);
      })
      .then(() => {
        return Promise.resolve()
          .then(() => (this.status = UploadStatus.ProcessingScorm))
          .then(() => this.uploaderService.processScormFilePackage(uploadParameters, false));
      })
      .then(() => {
        if (UploadCardComponent.graphicsExtensions.find(ext => ext === this.uploadParameters.fileExtension)) {
          this.updateValue(`${AppGlobal.environment.cloudfrontUrl}/${this.uploadParameters.fileLocation}`);
        }
        this.status = UploadStatus.Complete;
        this.moduleFacadeService.spinnerService.hide();
        this.uploaderProgressChange.emit({ status: UploadProgressStatus.Completed, parameters: this.uploadParameters });
      })
      .then(() => {
        this.showNotification(this.translate('The file is successfully uploaded'), NotificationType.Success);
        return this.uploadParameters.fileLocation
          ? `${AppGlobal.environment.cloudfrontUrl}/${this.uploadParameters.fileLocation}`
          : undefined;
      });
  }

  private updateValue(value: string | null): void {
    this.writeValue(value);
    if (this.onChange != null) {
      this.onChange(value);
    }
  }
}
