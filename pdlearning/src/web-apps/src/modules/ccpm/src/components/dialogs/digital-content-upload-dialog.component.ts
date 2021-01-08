import {
  AmazonS3UploaderService,
  BaseFormComponent,
  FileUploaderSetting,
  IMultipartFileInfo,
  IUploaderProgress,
  LocalTranslatorService,
  ModuleFacadeService,
  ScormProcessStatus,
  UploadProgressStatus
} from '@opal20/infrastructure';
import { Component, Input, TemplateRef, ViewChild } from '@angular/core';
import {
  ContentApiService,
  CopyrightLicenseTerritory,
  CopyrightLicenseType,
  CopyrightOwnership,
  DigitalUploadContentRequest,
  IDigitalContent
} from '@opal20/domain-api';
import { FileUploaderUtils, OpalFileUploaderComponent } from '@opal20/common-components';

import { DialogRef } from '@progress/kendo-angular-dialog';
import { DropDownListComponent } from '@progress/kendo-angular-dropdowns';

@Component({
  selector: 'app-digital-content-upload-dialog',
  templateUrl: './digital-content-upload-dialog.component.html'
})
export class DigitalContentUploadDialogComponent extends BaseFormComponent {
  public mode: 'upload' | 'create' = 'create';
  public thumbnailUrl: string;
  public zipPackages: IDataItem[] = [
    {
      text: 'SCORM',
      value: 'scorm'
    }
  ];
  public selectedZipPackage: IDataItem | undefined;
  public chooseZipTypeDialog: DialogRef;
  public confirmCancelDialogRef: DialogRef;

  public processScormStatus: string;
  public uploading: boolean;
  public isProcessingScormFile: boolean;

  @Input() public fileUploaderSetting: FileUploaderSetting;
  @Input() public canRunValidation: boolean = true;

  private fakeFileUploadModel: IDigitalContent = {
    title: 'Draft',
    type: 'UploadedContent',
    ownership: CopyrightOwnership.MoeOwned,
    licenseType: CopyrightLicenseType.Perpetual,
    termsOfUse: '',
    startDate: null,
    expiredDate: null,
    publisher: null,
    acknowledgementAndCredit: null,
    remarks: null,
    licenseTerritory: CopyrightLicenseTerritory.Singapore,
    isAllowReusable: false,
    isAllowDownload: false,
    isAllowModification: false,
    attributionElements: []
  };

  private wasCanceled: boolean = false;

  @ViewChild('zipPackageTemplate', { static: false })
  private zipPackageTemplate: TemplateRef<unknown>;
  @ViewChild('zipPackageControl', { static: false })
  private zipPackageControl: DropDownListComponent;
  @ViewChild('fileUploader', { static: true })
  private fileUploader: OpalFileUploaderComponent;

  constructor(
    protected moduleFacadeService: ModuleFacadeService,
    private digitalContentBackendService: ContentApiService,
    private dialogRef: DialogRef,
    private uploaderService: AmazonS3UploaderService,
    public translator: LocalTranslatorService
  ) {
    super(moduleFacadeService);
    this.fileUploaderSetting = new FileUploaderSetting();
    this.fileUploaderSetting.isFileLimitSizeValidate = this.canRunValidation;
    this.fileUploaderSetting.extensions = FileUploaderUtils.uploadContentAllowedExtensions;
    this.fileUploaderSetting.isShowExtensionsOnError = true;
  }

  public additionalValidationFn(): (i: IMultipartFileInfo) => Promise<IMultipartFileInfo> {
    return (info: IMultipartFileInfo) =>
      Promise.resolve().then(
        () =>
          new Promise((resolve: (info: IMultipartFileInfo) => void, reject: (msg?: string) => void) => {
            if (info.extension === 'zip') {
              this.chooseZipTypeDialog = this.moduleFacadeService.dialogService.open({
                content: this.zipPackageTemplate
              });

              return this.chooseZipTypeDialog.result.toPromise().then(result => {
                if (this.selectedZipPackage) {
                  info.extension = this.selectedZipPackage.value as string;
                  info.mimeType = `application/${this.selectedZipPackage.value}`;
                }

                return result === true ? resolve(info) : reject();
              });
            }

            return resolve(info);
          })
      );
  }

  public chooseZipPackage(): void {
    this.selectedZipPackage = this.zipPackageControl.value;
    this.chooseZipTypeDialog.close(true);
  }

  public closeDialog(): void {
    if (this.uploading) {
      this.confirmCancelDialogRef = this.modalService.showConfirmMessage(this.translate('Are you sure you want to cancel?'), () => {
        if (this.uploading) {
          this.wasCanceled = true;
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
        if (fileParameter.fileExtension === 'scorm') {
          this.moduleFacadeService.spinnerService.show();
          this.isProcessingScormFile = true;
          this.processScormStatus = this.translate('Processing your SCORM package');

          fileParameter.onChangeStatus = status => {
            switch (status) {
              case ScormProcessStatus.Timeout:
                this.showError('Uploading session timed out.');
                break;
              case ScormProcessStatus.Failure:
                this.showError('Could not process your file.');
                break;
              case ScormProcessStatus.Completed:
                this.processScormStatus = this.translate('Process completed.');
                break;
              default:
                this.processScormStatus = status;
                // Call show spinner here to avoid spinner get OFF due to it reached to maximum time
                this.moduleFacadeService.spinnerService.show();
                break;
            }
          };
        }
        if (this.mode === 'create') {
          this.fakeFileUploadModel.fileName = fileParameter.file.name;
          this.fakeFileUploadModel.fileExtension = fileParameter.fileExtension;
          this.fakeFileUploadModel.fileLocation = fileParameter.fileLocation;
          this.fakeFileUploadModel.fileSize = fileParameter.file.size;
          this.fakeFileUploadModel.fileType = fileParameter.mineType;
          this.uploaderService.processScormFilePackage(fileParameter, false).then(() => {
            if (!this.wasCanceled) {
              this.digitalContentBackendService
                .createDigitalContent(new DigitalUploadContentRequest(this.fakeFileUploadModel), false)
                .then(digitalContent => {
                  this.moduleFacadeService.spinnerService.hide();

                  this.dialogRef.close(digitalContent.id);
                  this.closeConfirmCancelDialog();
                })
                .catch(error => {
                  this.moduleFacadeService.spinnerService.hide();
                  this.showError(error || 'Create content failed');
                });
            }
          });
        } else {
          this.uploaderService.processScormFilePackage(fileParameter, false).then(() => {
            this.moduleFacadeService.spinnerService.hide();
            this.closeConfirmCancelDialog();
            this.dialogRef.close(fileParameter);
          });
        }
        break;
    }
  }

  private closeConfirmCancelDialog(): void {
    // if cancel confirm dialog is opening => close it
    if (this.confirmCancelDialogRef) {
      this.confirmCancelDialogRef.close();
    }
  }

  private showError(message?: string): void {
    message = message || 'There was a problem processing your file. Please try again';
    this.uploading = false;
    this.isProcessingScormFile = false;
    this.fileUploader.showError(this.translate(message));
    this.processScormStatus = null;
    this.moduleFacadeService.spinnerService.hide();
  }
}
